using UnityEngine;
using UnityEngine.Events;

namespace Erogemy.BlockBreaker.View
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] RectTransform rectTransform;

        public void SetPosition(Vector2 position)
        {
            var pos = position;
            pos.y = rectTransform.anchoredPosition.y; // Y座標は固定
            rectTransform.anchoredPosition = pos;
        }
    }
}
