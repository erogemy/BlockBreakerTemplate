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

        int currentPhase;
        Sequence currentSequence = Sequence.Title;
        int remainingBallCount;
        int remainingBlockCount;

        PhaseView[] phaseViews;

        void Awake()
        {
            // スタートボタンのリスナ
            mainUIView.OnClickScreen.AddListener(OnClick);
            // ボールが落ちた時のリスナ
            ball.OnBallFell.AddListener(OnBallFell);
            // ボールがブロックを消去した時のリスナ
            ball.OnRemoveBlock.AddListener(OnRemoveBlock);

            phaseViews = gameCanvas.GetComponentsInChildren<PhaseView>();
            mainUIView.UpdateContent(currentSequence);
        }

        void Start()
        {
            SetVisibleBallAndPaddle(false);
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
                    StartPhase(0);
                    break;
                case Sequence.WaitStart:
                    // ボールを発射してPlayingに
                    ball.Shoot(bockBreakerSettings.ballMoveSpeed);
                    currentSequence = Sequence.Playing;
                    break;
                case Sequence.Playing:
                    // 無視
                    break;
                case Sequence.PhaseClear:
                    if (phaseViews.Length > currentPhase + 1)
                    {
                        // 次のフェーズへ
                        StartPhase(currentPhase + 1);
                    }
                    else
                    {
                        // 全フェーズクリア
                        currentSequence = Sequence.Complete;
                    }
                    break;
                case Sequence.GameOver:
                    ResetBallAndPaddle();
                    ActivatePhaseView(0);
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

        void StartPhase(int phaseIndex)
        {
            currentPhase = phaseIndex;
            remainingBallCount = bockBreakerSettings.ballCount;
            ActivatePhaseView(phaseIndex);
            remainingBlockCount = phaseViews[phaseIndex].BlockContainer.Count;

            // ボールとパドルを表示
            SetVisibleBallAndPaddle(true);
            ResetBallAndPaddle();

            if (bockBreakerSettings.recoverBallOnPhaseClear)
            {
                remainingBallCount = bockBreakerSettings.ballCount;
            }

            mainUIView.SetBallCount(remainingBallCount);
            currentSequence = Sequence.WaitStart;
        }

        void ActivatePhaseView(int phaseIndex)
        {
            for (var i = 0; i < phaseViews.Length; i++)
            {
                phaseViews[i].SetActive(i == phaseIndex);
            }

            phaseViews[phaseIndex].ActivateAllBlocks();
        }

        // ボール落下
        void OnBallFell()
        {
            remainingBallCount--;
            if (remainingBallCount <= 0)
            {
                // ゲームオーバー
                currentSequence = Sequence.GameOver;
                SetVisibleBallAndPaddle(false);
            }
            else
            {
                currentSequence = Sequence.WaitStart;
                ResetBallAndPaddle();
                mainUIView.SetBallCount(remainingBallCount);
            }

            mainUIView.UpdateContent(currentSequence);
        }

        void OnRemoveBlock()
        {
            // 残りブロックカウントを減算
            remainingBlockCount--;
            // skipPhaseThresholdを超えたら次のフェーズへ
            if (remainingBlockCount > bockBreakerSettings.skipPhaseThreshold)
            {
                return;
            }

            // phaseに併せたブロックと背景を表示
            phaseViews[currentPhase].ShowCompleteView();
            // パドルを非表示
            SetVisibleBallAndPaddle(false);
            // プレイを一時停止
            ResetBallAndPaddle();

            currentSequence = Sequence.PhaseClear;
            mainUIView.UpdateContent(currentSequence);
        }

        void SetVisibleBallAndPaddle(bool isVisible)
        {
            ball.gameObject.SetActive(isVisible);
            paddle.gameObject.SetActive(isVisible);
        }

        void ResetBallAndPaddle()
        {
            // ボールとパドルの位置をリセット
            ball.ResetPosition();
            paddle.ResetPosition();
        }
    }
}
