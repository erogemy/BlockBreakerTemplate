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
            // pivotが左下であることを前提にoffset調整
            boxCollider.offset = size / 2;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
