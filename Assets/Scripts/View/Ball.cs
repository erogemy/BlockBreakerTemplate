using UnityEngine;
using UnityEngine.Events;

namespace Erogemy.BlockBreaker.View
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] Rigidbody2D ballRb;
        [SerializeField] RectTransform rectTransform;

        Vector2 initPos;
        float reflectionPower;

        bool isReflectionInFrame;
        Vector2? lastBlockDiffInFrame; // 最後に接触したブロックとのdiff

        public UnityEvent OnBallFell { get; } = new();
        public UnityEvent OnRemoveBlock { get; } = new();

        void Start()
        {
            initPos = rectTransform.anchoredPosition;
        }

        void FixedUpdate()
        {
            isReflectionInFrame = false;
            lastBlockDiffInFrame = null;
        }

        public void Init(float reflectionPower)
        {
            this.reflectionPower = reflectionPower;
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

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.TryGetComponent<Paddle>(out var paddle))
            {
                if (isReflectionInFrame)
                {
                    return;
                }

                var normalPos = paddle.GetNormalPosition(rectTransform.anchoredPosition);

                var velocity = new Vector2(ballRb.linearVelocity.x, -ballRb.linearVelocity.y);
                if (Mathf.Abs(normalPos) < 0.1f)
                {
                    // パドルの中央に近い場合は、純粋な反射
                    ballRb.linearVelocity = velocity;
                    return;
                }

                // normalPosを-20度~+20度の範囲に変換する
                var velX = velocity.x + normalPos * reflectionPower * velocity.magnitude;
                var newDirection = new Vector2(velX, velocity.y).normalized;

                // -80度~80度を超えないように調整
                const float maxReflectionAngle = 80f; // 最大反射角度
                newDirection = ApplyAngleLimit(newDirection, maxReflectionAngle);

                ballRb.linearVelocity = newDirection * velocity.magnitude;
                isReflectionInFrame = true;
            }
            else if (collider.TryGetComponent<FallArea>(out _))
            {
                OnBallFell.Invoke();
            }
        }


        Vector2 ApplyAngleLimit(Vector2 direction, float maxLimitAngle)
        {
            var angle = Vector2.Angle(Vector2.up, direction);

            if (angle <= maxLimitAngle)
            {
                return direction.normalized;
            }
            // 角度を制限値に修正
            var sign = Mathf.Sign(direction.x);
            var limitedAngleRad = maxLimitAngle * Mathf.Deg2Rad;

            // 制限角度でのベクトルを計算
            var x = Mathf.Sin(limitedAngleRad) * sign;
            var y = Mathf.Cos(limitedAngleRad);

            return new Vector2(x, y).normalized;
        }

        public void SetPositionX(float positionX)
        {
            var pos = rectTransform.anchoredPosition;
            pos.x = positionX;
            rectTransform.anchoredPosition = pos;
        }

        public Vector2 GetPosition()
        {
            return rectTransform.anchoredPosition;
        }

        public void OnCollisionBlock(Vector2 velocity, Vector2 blockPosition)
        {
            OnRemoveBlock.Invoke();
            var diff = blockPosition - rectTransform.anchoredPosition;
            if (lastBlockDiffInFrame.HasValue && lastBlockDiffInFrame.Value.sqrMagnitude < diff.sqrMagnitude)
            {
                // 前回のブロックより遠ければ無視
                return;
            }
            lastBlockDiffInFrame = diff;
            ballRb.linearVelocity = velocity;
        }
    }
}
