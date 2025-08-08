using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Sprites;

namespace Erogemy.BlockBreaker.Editor
{
    public class SpriteSliceWindow : EditorWindow
    {
        //現在選択中のテクスチャ
        Texture2D _texture;

        //分割する時の各スプライトのサイズ
        int _width = 16, _height = 16;

        [MenuItem("Tools/Erogemy/BlockBreaker/SpriteSliceWindow")]
        public static void Open()
        {
            CreateInstance<SpriteSliceWindow>().ShowUtility();
        }

        void OnGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                _texture = EditorGUILayout.ObjectField("Texture", _texture, typeof(Texture2D), false) as Texture2D;
                EditorGUILayout.LabelField($"Texture : {(_texture != null ? _texture.name : string.Empty)}");
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            {
                var size = new Vector2(_width, _height);
                size = EditorGUILayout.Vector2Field("Grid size", size);

                _width = (int)size.x;
                _height = (int)size.y;
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("ToSliceSprite") && _texture != null)
            {
                ToSliceSprite(_texture, new Vector2Int(_width, _height));
            }
        }

        static bool CheckContainOpacity(Texture2D texture, SpriteRect rect)
        {
            // 該当のrectが不透明pixelを含めばtrue
            var pixels = texture.GetPixels((int)rect.rect.x, (int)rect.rect.y, (int)rect.rect.width, (int)rect.rect.height);
            return pixels.Any(pixel => pixel.a != 0);
        }

        //スブライトをスライスする
        public static void ToSliceSprite(Texture2D texture, Vector2Int gridSize)
        {
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            var dataProvider = factory.GetSpriteEditorDataProviderFromObject(texture);
            dataProvider.InitSpriteEditorDataProvider();

            var importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
            var isReadable = importer.isReadable;
            {
                importer.spriteImportMode = SpriteImportMode.Multiple;
                // read/writeを有効にする
                importer.isReadable = true;
                importer.SaveAndReimport();
            }

            var metaDataList = new List<SpriteRect>();
            var yIndex = 0;
            for (var y = 0; y < texture.height; y += gridSize.y)
            {
                var xIndex = 0;
                for (var x = 0; x < texture.width; x += gridSize.x)
                {
                    //はみ出る場合は作成しない
                    if (x + gridSize.x > texture.width || y + gridSize.y > texture.height)
                    {
                        continue;
                    }

                    var smd = new SpriteRect
                    {
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = SpriteAlignment.Custom,
                        name = $"{texture.name}_{yIndex}_{xIndex}",
                        rect = new Rect(x, y, gridSize.x, gridSize.y)
                    };

                    // 分割しようとしたpixelが不透明要素を含めば追加
                    var containOpacity = CheckContainOpacity(texture, smd);
                    if (containOpacity)
                    {
                        metaDataList.Add(smd);
                    }

                    ++xIndex;
                }

                ++yIndex;
            }

            dataProvider.SetSpriteRects(metaDataList.ToArray());

            dataProvider.Apply();

            // アセットを再インポートして、スプライトを更新
            importer.isReadable = isReadable;
            importer.SaveAndReimport();
        }
    }
}
