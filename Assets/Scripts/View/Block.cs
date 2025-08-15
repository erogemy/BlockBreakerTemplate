using UnityEngine;
using UnityEngine.UI;

namespace Erogemy.BlockBreaker.View
{
    public class Block : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] BoxCollider2D boxCollider;

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetPositionAndSize(Vector2 pos,Vector2 size)
        {
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = pos;

            boxCollider.size = size;

            // 中央に変更(テンプレprefabは左下pivot)
            var globalPos = transform.position;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            transform.position = globalPos;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        public Vector2 GetPosition()
        {
            // 親のスケールを考慮
            var globalScale = image.rectTransform.lossyScale;
            return image.rectTransform.anchoredPosition * globalScale;
        }

        public Vector2 GetSize()
        {
            // 親のスケールを考慮
            var globalScale = image.rectTransform.lossyScale;
            return boxCollider.size * globalScale;
        }
    }
}
