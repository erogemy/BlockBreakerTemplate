using System.IO;
using Erogemy.BlockBreaker.View;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Erogemy.BlockBreaker.Editor
{
    public static class BlockBreakerBuilder
    {
        public static void Build(int blockSize)
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
            SetupScene(phaseCount);
        }

        static void SetupScene(int phaseCount)
        {
            var gameCanvas = GameObject.Find("GameCanvas");
            var phasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(EditorConsts.LocalPackagePath+EditorConsts.PhaseTemplatePath);

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
                SetupBlockPrefabs();
                // GameCanvas内のヒエラルキー順を先頭に(Phase_1が最前面に来てほしい)
                phaseObject.transform.SetAsFirstSibling();

                // PresenterにPhaseを設定
            }

            // 高さが1920となるようにPlayAreaのRectTransformのwidthを調整
            var playArea = GameObject.Find("PlayArea");

            // widthとheightからアス比を計算
            var aspectRatio = (float)width / height;
            playArea.GetComponent<RectTransform>().sizeDelta = new Vector2(1080f * aspectRatio, 1080f);
        }

        static void SetupBlockPrefabs()
        {
            // Blockイメージを取得

            // Blockプレファブを取得
            // BlockプレファブのサイズをblockSizeに設定
            // BlockプレファブのBlockコンポーネントにBlockImageを設定
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
            if (!AssetDatabase.CopyAsset(EditorConsts.LocalPackagePath + EditorConsts.BlockBreakerTemplatePath, EditorConsts.BlockBreakerScenePath))
            {
                Debug.LogError("BlockBreakerシーンのコピーに失敗しました。");
                return false;
            }

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(EditorConsts.BlockBreakerScenePath);

            return true;
        }

        static void SetupBlockImage(int phaseCount, int blockSize)
        {
            for (var i = 0; i < phaseCount; i++)
            {
                // フォルダ命名は1オリジン
                var path = $"{EditorConsts.ImagesPath}Phase_{i + 1}/{EditorConsts.BlockImageName}";
                var image = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                SpriteSliceWindow.ToSliceSprite(image, Vector2Int.one * blockSize);
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
    }
}
