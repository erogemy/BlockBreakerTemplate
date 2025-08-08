using Erogemy.BlockBreaker.Model;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Erogemy.BlockBreaker.View
{
    public class MainUIView : MonoBehaviour
    {
        [SerializeField] GameObject titleMessage;
        [SerializeField] GameObject phaseSuccessMessage;
        [SerializeField] GameObject completeMessage;
        [SerializeField] GameObject startPlayMessage;
        [SerializeField] GameObject gameOverMessage;
        [SerializeField] TMP_Text remainingBallText;
        [SerializeField] Button fullScreenButton;

        public UnityEvent OnClickScreen => fullScreenButton.onClick;

        public void SetBallCount(int count)
        {
            remainingBallText.text = $"Life: {count}";
        }

        public void UpdateContent(Sequence sequence)
        {
            titleMessage.SetActive(sequence == Sequence.Title);
            phaseSuccessMessage.SetActive(sequence == Sequence.PhaseClear);
            completeMessage.SetActive(sequence == Sequence.Complete);
            startPlayMessage.SetActive(sequence == Sequence.WaitStart);
            gameOverMessage.SetActive(sequence == Sequence.GameOver);
        }
    }
}
