using UnityEngine;

namespace Erogemy.BlockBreaker.View
{
    public class Paddle : MonoBehaviour
    {
        [SerializeField] RectTransform rectTransform;
        Vector2 initPos;

        public float Width => rectTransform.rect.width;

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

        public float GetNormalPosition(Vector2 ballAnchorPosition)
        {
            // パドル自身の横幅を-1 ~ 1として、ballAnchorPosition.Xの値を正規化する
            var paddleWidth = rectTransform.rect.width;

            // パドルの中心位置を取得
            var paddleCenterX = rectTransform.anchoredPosition.x;

            var normalizedPosition = (ballAnchorPosition.x - paddleCenterX) / (paddleWidth / 2);
            // -1 ~ 1の範囲に収める
            return Mathf.Clamp(normalizedPosition, -1f, 1f);
        }

        public Vector2 GetAnchorPosition()
        {
            return rectTransform.anchoredPosition;
        }
    }
}
