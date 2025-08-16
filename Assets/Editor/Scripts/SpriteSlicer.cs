using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Sprites;

namespace Erogemy.BlockBreaker.Editor
{
    public class SpriteSlicer
    {
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

            var rawScale = 1.0f / CalcRawTexScale(importer);
            dataProvider.SetSpriteRects(GenerateSliceInfo(texture, gridSize, GetRawTextureSize(importer),rawScale).ToArray());

            dataProvider.Apply();

            // アセットを再インポートして、スプライトを更新
            importer.isReadable = isReadable;
            importer.SaveAndReimport();
        }

        static Vector2Int GetRawTextureSize(TextureImporter importer)
        {
            importer.GetSourceTextureWidthAndHeight(out var rawWidth, out var rawHeight);
            return new Vector2Int(rawWidth, rawHeight);
        }

        static float CalcRawTexScale(TextureImporter importer)
        {
            var maxImportSize = importer.maxTextureSize;
            importer.GetSourceTextureWidthAndHeight(out var rawWidth, out var rawHeight);

            if (rawWidth <= maxImportSize && rawHeight <= maxImportSize)
            {
                return 1;
            }

            return (float)Mathf.Max(rawWidth, rawHeight) / maxImportSize;
        }

        static List<SpriteRect> GenerateSliceInfo(Texture2D texture, Vector2Int gridSize, Vector2Int rawTexSize, float rawScale)
        {
            var metaDataList = new List<SpriteRect>();
            var yIndex = 0;
            for (var y = 0; y < rawTexSize.y; y += gridSize.y)
            {
                var xIndex = 0;
                for (var x = 0; x < rawTexSize.x; x += gridSize.x)
                {
                    //はみ出る場合は作成しない
                    if (x + gridSize.x > rawTexSize.x || y + gridSize.y > rawTexSize.y)
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
                    var containOpacity = CheckContainOpacity(texture, smd, rawScale);
                    if (containOpacity)
                    {
                        metaDataList.Add(smd);
                    }

                    ++xIndex;
                }

                ++yIndex;
            }
            return metaDataList;
        }

        static bool CheckContainOpacity(Texture2D texture, SpriteRect rect, float scale)
        {
            // 該当のrectが不透明pixelを含めばtrue
            var pixels = texture.GetPixels(
                (int)(rect.rect.x * scale),
                (int)(rect.rect.y * scale),
                (int)(rect.rect.width* scale),
                (int)(rect.rect.height * scale));
            return pixels.Any(pixel => pixel.a != 0);
        }
    }
}
