using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Erogemy.BlockBreaker.Editor
{
    public static class ResourcesValidator
    {
        // 指定の画像配置用フォルダの中にPhase_Xとなっているディレクトリが連番でいくつあるか確認
        // ValidなPhaseの数を返す
        public static int ValidateAndGetPhaseCount(out string message)
        {
            var directories = System.IO.Directory.GetDirectories(EditorConsts.ImagesPath, "Phase_*");

            var count = 0;
            for (; count < directories.Length; count++)
            {
                if (!directories.Contains(EditorConsts.ImagesPath + $"Phase_{count + 1}"))
                {
                    break;
                }
            }

            if (count == 0) // 一度もforが回らなかった
            {
                message = $"{EditorConsts.ImagesPath}にPhase_1フォルダを作成してください";
                return 0;
            }

            if (!ValidateImageSize(count, out message))
            {
                return 0;
            }

            message = null;
            return count;
        }

        static bool ValidateImageSize(int count, out string message)
        {
            var width = -1;
            var height = -1;
            for (var i = 1; i <= count; i++)
            {
                var baseImagePath = $"{EditorConsts.ImagesPath}Phase_{i}/{EditorConsts.BaseImageName}";
                if (!ValidateImage(baseImagePath, ref width, ref height, out message))
                {
                    return false;
                }

                var blockImagePath = $"{EditorConsts.ImagesPath}Phase_{i}/{EditorConsts.BlockImageName}";
                if (!ValidateImage(blockImagePath, ref width, ref height, out message))
                {
                    return false;
                }
            }

            message = null;
            return true;
        }

        static bool ValidateImage(string path, ref int width, ref int height, out string message)
        {
            var image = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (image == null)
            {
                message = $"{path}を配置してください";
                return false;
            }

            if (width == -1 && height == -1)
            {
                width = image.width;
                height = image.height;
            }
            else if (width != image.width || height != image.height)
            {
                message = $"{path}のサイズが他の画像と異なります。解像度を揃えてください";
                return false;
            }

            message = null;
            return true;
        }
    }
}
