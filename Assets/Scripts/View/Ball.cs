using UnityEngine;
using UnityEngine.Events;

namespace Erogemy.BlockBreaker.View
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] Rigidbody2D ballRb;
        [SerializeField] RectTransform rectTransform;
        Vector2 initPos;

        public UnityEvent OnBallFell { get; } = new();
        public UnityEvent OnRemoveBlock { get; } = new();

        void Start()
        {
            initPos = rectTransform.anchoredPosition;
        }

        public void ResetPosition()
        {
            rectTransform.anchoredPosition = initPos;
            ballRb.linearVelocity = Vector2.zero;
        }

        public void Shoot(float speed)
        {
            // 適当な角度に射出
            ballRb.linearVelocity = new Vector2(5f, 5f) * speed;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent<Block>(out var block))
            {
                OnRemoveBlock.Invoke();
                block.SetActive(false);
            }
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<Paddle>(out _))
            {
                // パドルの中央に近いほど純粋な反射にする
                // パドルの端に近いほど角度をつけて跳ね返す
                var paddlePos = collider.transform.position;
                var paddleSize = collider.GetComponent<RectTransform>().sizeDelta;
                var contactPoint = transform.position - paddlePos;
                // パドルの中央からの距離を計算
                var distanceFromCenter = Mathf.Abs(contactPoint.x - paddleSize.x / 2);
                // 跳ね返る角度を計算
                var angle = Mathf.Clamp(distanceFromCenter / (paddleSize.x / 2), 0.1f, 1f) * Mathf.PI / 4; // 45
                // 反射ベクトルを計算
                var reflectionVector = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                // ボールの速度を更新
                ballRb.linearVelocity = reflectionVector * ballRb.linearVelocity.magnitude;
            }
            else if (collider.TryGetComponent<FallArea>(out _))
            {
                OnBallFell.Invoke();
            }
        }
    }
}
