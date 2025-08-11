using UnityEngine;
using UnityEngine.Events;

namespace Erogemy.BlockBreaker.View
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] RectTransform rectTransform;
        Vector2 initPos;

        void Awake()
        {
            initPos = rectTransform.anchoredPosition;
        }

        public void ResetPosition()
        {
            rectTransform.anchoredPosition = initPos;
        }

        public void SetPosition(Vector2 position)
        {
            var pos = position;
            pos.y = rectTransform.anchoredPosition.y; // Y座標は固定
            rectTransform.anchoredPosition = pos;
        }
    }
}
