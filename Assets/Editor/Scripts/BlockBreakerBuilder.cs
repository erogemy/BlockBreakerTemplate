using System;
using System.IO;
using System.Linq;
using Erogemy.BlockBreaker.Model;
using Erogemy.BlockBreaker.Presenter;
using Erogemy.BlockBreaker.View;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Erogemy.BlockBreaker.Editor
{
    public static class BlockBreakerBuilder
    {
        public static void Build(Vector2Int blockSize, BockBreakerSettings settings)
        {
            if (!CreateScene())
            {
                return;
            }

            var phaseCount = ResourcesValidator.ValidateAndGetPhaseCount(out var message);
            if (phaseCount == 0)
            {
                Debug.LogError(message);
                return;
            }

            SetupBlockImage(phaseCount, blockSize);
            SetupScene(phaseCount, blockSize, settings);
        }

        static void SetupScene(int phaseCount, Vector2Int blockSize, BockBreakerSettings settings)
        {
            var gameCanvas = Object.FindAnyObjectByType<GameCanvasView>();
            ;
            var phasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorConsts.PackagePath + EditorConsts.PhaseTemplatePath);

            var width = 0;
            var height = 0;

            for (var i = 0; i < phaseCount; i++)
            {
                // Phase_Templateをシーン上に生成
                var phaseObject = PrefabUtility.InstantiatePrefab(phasePrefab, gameCanvas.transform) as GameObject;
                phaseObject.name = $"Phase_{i + 1}";

                // Phase_XにBaseイメージを設定
                var baseImagePath = $"{EditorConsts.ImagesPath}Phase_{i + 1}/{EditorConsts.BaseImageName}";
                var baseImage = AssetDatabase.LoadAssetAtPath<Texture2D>(baseImagePath);

                // PlayAreaのためにサイズを覚えておく
                width = baseImage.width;
                height = baseImage.height;

                var component = phaseObject.GetComponent<PhaseView>();
                component.SetBaseImage(Sprite.Create(baseImage, new Rect(0, 0, baseImage.width, baseImage.height), Vector2.zero));

                // Phase_XのBlocksにblockプレファブを並べていく
                SetupBlockPrefabs(i, component);
                // GameCanvas内のヒエラルキー順を先頭に(Phase_1が最前面に来てほしい)
                phaseObject.transform.SetAsFirstSibling();
            }

            // 高さが1920となるようにPlayAreaのRectTransformのwidthを調整
            var playArea = GameObject.Find("PlayArea");

            // widthとheightからアス比を計算
            var aspectRatio = (float)width / height;
            playArea.GetComponent<RectTransform>().sizeDelta = new Vector2(1080f * aspectRatio, 1080f);

            Object.FindAnyObjectByType<BlockBreakerSamplePresenter>().ApplySettings(settings);
        }

        static void SetupBlockPrefabs(int phase, PhaseView parentPhase)
        {
            // Blockイメージを取得
            var blockImagePath = $"{EditorConsts.ImagesPath}Phase_{phase + 1}/{EditorConsts.BlockImageName}";
            var blockImage = AssetDatabase.LoadAllAssetsAtPath(blockImagePath).OfType<Sprite>();

            // Blockプレファブを取得
            var blockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorConsts.PackagePath + EditorConsts.BlockTemplatePath);
            var container = parentPhase.BlockContainer;

            var blockCount = 0;
            foreach (var cellImage in blockImage)
            {
                var block = PrefabUtility.InstantiatePrefab(blockPrefab, container.transform) as GameObject;
                blockPrefab.name = $"Block_{blockCount + 1}";
                var blockComponent = block.GetComponent<Block>();

                // BlockプレファブのBlockコンポーネントにBlockImageを設定
                blockComponent.SetImage(cellImage);
                var blockSize = new Vector2(cellImage.rect.width, cellImage.rect.height);

                // baseImageはBlock_Y_Xという名前なのでXYを取り出してVector2に詰める
                var nameParts = cellImage.name.Split('_');
                var blockIndexVector = new Vector2(int.Parse(nameParts[2]), int.Parse(nameParts[1]));
                blockComponent.SetPositionAndSize(blockIndexVector * blockSize, blockSize);

                blockCount++;
            }
        }

        static bool CreateScene()
        {
            // すでにシーンが存在する場合はそのまま
            if (SceneManager.GetSceneByPath(EditorConsts.BlockBreakerScenePath).IsValid())
            {
                if (!EditorUtility.DisplayDialog("シーンの上書き確認", "BlockBreakerシーンはすでに存在します。上書きしますか？", "はい", "いいえ"))
                {
                    return false;
                }

                // シーンを開いておく
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorConsts.BlockBreakerScenePath);
                return true;
            }

            CreatePath(EditorConsts.BlockBreakerScenePath);

            // Templateシーンをコピーして新規作成
            if (!AssetDatabase.CopyAsset(EditorConsts.PackagePath + EditorConsts.BlockBreakerTemplatePath, EditorConsts.BlockBreakerScenePath))
            {
                Debug.LogError("BlockBreakerシーンのコピーに失敗しました。");
                return false;
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorConsts.BlockBreakerScenePath);

            return true;
        }

        static void SetupBlockImage(int phaseCount, Vector2Int blockSize)
        {
            for (var i = 0; i < phaseCount; i++)
            {
                // フォルダ命名は1オリジン
                var path = $"{EditorConsts.ImagesPath}Phase_{i + 1}/{EditorConsts.BlockImageName}";
                var image = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                SpriteSlicer.ToSliceSprite(image, blockSize);
            }
        }

        static void CreatePath(string path)
        {
            var directoryPath = Path.GetDirectoryName(path);
            if (Directory.Exists(directoryPath))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(directoryPath);
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create directory {directoryPath}: {e.Message}");
            }
        }

        public static void BuildScene()
        {
            var scene = SceneManager.GetActiveScene();
            if (!scene.IsValid())
            {
                Debug.LogError("現在のシーンが無効です");
            }

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL))
                {
                    Debug.LogError("WebGLビルドターゲットへの切り替えに失敗しました。UnityHubで「Web」サポートがインストールされているか確認してください");
                    return;
                }
            }

            var path = EditorUtility.OpenFolderPanel("フォルダを選択", "", "");

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { scene.path },
                locationPathName = path,
                target = BuildTarget.WebGL,
                targetGroup = BuildTargetGroup.WebGL
            };
            var options = BuildOptions.None;
            buildPlayerOptions.options = options;
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
            PlayerSettings.SetIl2CppCodeGeneration(
                NamedBuildTarget.WebGL,
                Il2CppCodeGeneration.OptimizeSize // コードが小さいのでランタイム速度を気にせずビルド時間を短く
            );
            EditorSceneManager.SaveScene(scene, EditorConsts.BlockBreakerScenePath);
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log($"{path}にビルド成果物を配置しました！");
        }
    }
}
