using UnityEngine;

namespace Erogemy.BlockBreaker.View
{
    public class GameCanvasView : MonoBehaviour
    {
        PhaseView[] phaseViews;

        public int PhaseCount => phaseViews.Length;

        void Awake()
        {
            phaseViews = GetComponentsInChildren<PhaseView>();
        }

        public int GetPhaseBlockCount(int phaseIndex)
        {
            return phaseViews[phaseIndex].BlockContainer.Count;
        }

        public void ShowCompleteView(int phaseIndex)
        {
            phaseViews[phaseIndex].ShowCompleteView();
        }

        public void ActivatePhaseView(int phaseIndex)
        {
            for (var i = 0; i < phaseViews.Length; i++)
            {
                phaseViews[phaseIndex].SetActive(i == phaseIndex);
            }

            phaseViews[phaseIndex].ActivateAllBlocks();
        }
    }
}
