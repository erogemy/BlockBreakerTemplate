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

        void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent<Block>(out var block))
            {
                return;
            }

            OnRemoveBlock.Invoke();
            block.SetActive(false);

            if (isReflectionInFrame)
            {
                return;
            }

            var diff = rectTransform.anchoredPosition - block.GetPosition();
            if (lastBlockDiffInFrame != null)
            {
                if (lastBlockDiffInFrame.Value.magnitude < diff.magnitude)
                {
                    // 前回のブロックとの接触の方が近ければ無視
                    return;
                }
            }
            lastBlockDiffInFrame = diff;

            // ボールの速度をブロックと相対で取る(ブロック目線なので*-1しとく)
            var relativeVelocity = -other.relativeVelocity;
            var surface = CalcCollisionSurface(
                rectTransform.anchoredPosition,
                block.GetPosition(),
                block.GetSize(),
                relativeVelocity
            );

            if (surface.x != 0 && surface.y != 0)
            {
                // 角に衝突した時はどの角かで反射を制御
                switch (surface.x)
                {
                    case < 0 when surface.y < 0:
                        // 左下
                        ballRb.linearVelocity　= new Vector2(
                            - Mathf.Abs(relativeVelocity.x),
                            - Mathf.Abs(relativeVelocity.y));
                        break;
                    case < 0 when surface.y > 0:
                        // 左上
                        ballRb.linearVelocity　= new Vector2(
                            - Mathf.Abs(relativeVelocity.x),
                            Mathf.Abs(relativeVelocity.y));
                        break;
                    case > 0 when surface.y < 0:
                        // 右下
                        ballRb.linearVelocity　= new Vector2(
                            Mathf.Abs(relativeVelocity.x),
                            - Mathf.Abs(relativeVelocity.y));
                        break;
                    default:
                        // 右上
                        ballRb.linearVelocity　= new Vector2(
                            Mathf.Abs(relativeVelocity.x),
                            Mathf.Abs(relativeVelocity.y));
                        break;
                }
            }
            else
            {
                ballRb.linearVelocity = relativeVelocity * new Vector2(
                    surface.x != 0 ? -1 :1,
                    surface.y != 0 ? -1 :1);
            }
        }

        static Vector2 CalcCollisionSurface(Vector2 ballPosition, Vector2 blockPosition, Vector2 blockSize, Vector2 relativeVec)
        {
            // 対角線とdiffベクトルを比較してどの領域にいるかを判断
            var diff = ballPosition - blockPosition;

            // relativeVecの方向から考えられる衝突面を絞る
            if (Mathf.Abs(blockSize.x) * 0.5f > Mathf.Abs(diff.x))
            {
                return new Vector2(0, relativeVec.y > 1 ? -1 : 1); // 上か下
            }
            else if (Mathf.Abs(blockSize.y) * 0.5f > Mathf.Abs(diff.y))
            {
                return new Vector2(relativeVec.x > 1 ? -1 : 1, 0); // 左か右
            }

            // 角の4つ
            return new Vector2(
                diff.x > 0 ? 1 : -1,
                diff.y > 0 ? 1 : -1
            );
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
    }
}
