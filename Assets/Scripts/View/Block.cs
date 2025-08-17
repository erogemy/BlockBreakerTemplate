using System;
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

        Vector2 GetPosition()
        {
            // 親のスケールを考慮
            var globalScale = image.rectTransform.lossyScale;
            return image.rectTransform.anchoredPosition * globalScale;
        }

        Vector2 GetSize()
        {
            // 親のスケールを考慮
            var globalScale = image.rectTransform.lossyScale;
            return boxCollider.size * globalScale;
        }

        void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.TryGetComponent<Ball>(out var ball))
            {
                return;
            }

            SetActive(false);

            // ボールの速度をブロックと相対で取る
            var relativeVelocity = other.relativeVelocity;

            var surface = CalcCollisionSurface(
                GetPosition(),
                ball.GetPosition(),
                GetSize(),
                relativeVelocity
            );

            ball.OnCollisionBlock(
        Vector2.Reflect(relativeVelocity, surface),
               GetPosition()
            );
        }

        // 衝突面の法線を返す
        static Vector2 CalcCollisionSurface(Vector2 blockPosition, Vector2 ballPosition, Vector2 blockSize, Vector2 relativeVec)
        {
            // relativeVecで2面に絞れる
            var xSign = Math.Sign(relativeVec.x);
            var ySign = Math.Sign(relativeVec.y);

            // ゼロ方向があれば確定させちゃう
            if (xSign == 0 || ySign == 0)
            {
                return new Vector2(-xSign, -ySign);
            }

            var diff = ballPosition - blockPosition;
            Vector2 direction;
            Vector2 dirOffset;
            if (IsInBox(diff, blockSize))
            {
                // ブロック内にめり込んでいる場合は対角線にあたるベクトル
                direction = new Vector2(blockSize.x * xSign, blockSize.y * ySign);
                dirOffset = Vector2.zero;
            }
            else
            {
                // そうでなければ頂点を通る45度のベクトル
                direction = new Vector2(Math.Sign(diff.x), Math.Sign(diff.y));
                dirOffset = new Vector2(blockSize.x * 0.5f * -xSign, blockSize.y * 0.5f * -ySign);
            }

            var crossProduct = CalcPointSide(
                diff,
                dirOffset,
                direction
            );

            switch (xSign)
            {
                case < 0 when ySign > 0:
                    // 左上への進入
                    return crossProduct > 0
                        ? Vector2.right // 右壁
                        : Vector2.down; // 下壁
                case < 0 when ySign < 0:
                    // 左下への進入
                    return crossProduct > 0
                        ? Vector2.up
                        : Vector2.right;
                case > 0 when ySign < 0:
                    // 右下への進入
                    return crossProduct > 0
                        ? Vector2.left
                        : Vector2.up;
                default:
                    // 右上への進入(のはず)
                    return crossProduct > 0
                        ? Vector2.down
                        : Vector2.left;
            }
        }

        // pVからのベクトルvに対して、点pが左側、右側、または直線上にあるかを判定
        // retval > 0: 左側, < 0: 右側, = 0: 直線上
        static float CalcPointSide(Vector2 p, Vector2 pV, Vector2 v)
        {
            var toPoint = p - pV;
            return v.x * toPoint.y - v.y * toPoint.x;
        }

        static bool IsInBox(Vector2 pos, Vector2 size)
        {
            var halfSize = size * 0.5f;
            return -halfSize.x < pos.x && pos.x < halfSize.x &&
                   -halfSize.y < pos.y && pos.y < halfSize.y;
        }
    }
}
