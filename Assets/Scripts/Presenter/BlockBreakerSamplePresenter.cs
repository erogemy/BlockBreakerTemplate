using System;
using Erogemy.BlockBreaker.Model;
using Erogemy.BlockBreaker.View;
using UnityEngine;

namespace Erogemy.BlockBreaker.Presenter
{
    public class BlockBreakerSamplePresenter : MonoBehaviour
    {
        [SerializeField] BockBreakerSettings bockBreakerSettings;
        [SerializeField] MainUIView mainUIView;
        [SerializeField] GameObject gameCanvas;
        [SerializeField] Ball ball;
        [SerializeField] Paddle paddle;
        [SerializeField] PlayerInputWrapper playerInput;

        int currentPhase = 0;
        Sequence currentSequence = Sequence.Title;

        PhaseView[] phaseViews;

        void Awake()
        {
            // スタートボタンのリスナ
            mainUIView.OnClickScreen.AddListener(OnClick);
            // ボールが落ちた時のリスナ
            ball.OnBallFell.AddListener(OnBallFell);
            // ボールがブロックを消去した時のリスナ
            ball.OnRemoveBlock.AddListener(OnRemoveBlock);

            phaseViews = GetComponentsInChildren<PhaseView>();
            mainUIView.UpdateContent(currentSequence);
        }

        void Update()
        {
            if (currentSequence == Sequence.Playing || currentSequence == Sequence.WaitStart)
            {
                // プレイヤーの入力を受け付ける
                // パドルを動かす
                paddle.SetPosition(playerInput.TouchScreenPosition);
            }

            if (currentSequence == Sequence.WaitStart)
            {

                // 非ゲーム中であればボールをパドルに追従
            }
        }

        void OnClick()
        {
            switch (currentSequence)
            {
                case Sequence.Title:
                    // ボールとパドルを表示してWaitStartに
                    SetVisibleBallAndPaddle(true);
                    currentSequence = Sequence.WaitStart;
                    break;
                case Sequence.WaitStart:
                    // ボールを発射してPlayingに
                    break;
                case Sequence.Playing:
                    // 無視
                    break;
                case Sequence.PhaseClear:
                    // Completeか、そうでなければPhaseを入れ替えてWaitStartに
                    break;
                case Sequence.GameOver:
                    // Titleに
                    currentSequence = Sequence.Title;
                    break;
                case Sequence.Complete:
                    // 無視
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            mainUIView.UpdateContent(currentSequence);
        }

        // ボール落下
        void OnBallFell()
        {
            // ボールカウントがなければ専用メニューを出す
            // プレイボタンを一旦停止
            // ボールの位置をリセット

        }

        void OnRemoveBlock()
        {
            // 残りブロックカウントを減算
            // skipPhaseThresholdを超えたら次のフェーズへ
            return;
            // phaseに併せたブロックと背景を表示
            // パドルを非表示
            SetVisibleBallAndPaddle(false);
            // プレイを一時停止
            currentSequence = Sequence.PhaseClear;
        }

        void SetVisibleBallAndPaddle(bool isVisible)
        {
            ball.gameObject.SetActive(isVisible);
            paddle.gameObject.SetActive(isVisible);
        }

        public void SetSettings(BockBreakerSettings settings)
        {
            bockBreakerSettings = settings;
        }
    }
}
