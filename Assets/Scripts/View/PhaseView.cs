using UnityEngine;
using UnityEngine.UI;

namespace Erogemy.BlockBreaker.View
{
    public class PhaseView : MonoBehaviour
    {
        [SerializeField] Image baseImage;
        [SerializeField] BlockContainer blockContainer;

        public BlockContainer BlockContainer => blockContainer;

        public void SetBaseImage(Sprite sprite)
        {
            baseImage.sprite = sprite;
            baseImage.rectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);

            // 1920x1080の解像度で表示するためにスケールを調整
            var scaleX = 1920f / sprite.texture.width;
            var scaleY = 1080f / sprite.texture.height;
            var scale = Mathf.Min(scaleX, scaleY);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public void ShowCompleteView()
        {
            blockContainer.gameObject.SetActive(false);
        }

        public void ActivateAllBlocks()
        {
            blockContainer.SetActiveAll(true);
        }
    }
}
