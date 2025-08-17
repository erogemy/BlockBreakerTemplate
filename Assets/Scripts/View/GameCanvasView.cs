using System;
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
            Array.Sort(phaseViews, (x, y) =>
            {
                // "Phase_X"のX部分を数値化して比較
                var viewIndex = int.Parse(x.name.Split('_')[1]);
                var phaseViewIndex = int.Parse(y.name.Split('_')[1]);
                return viewIndex.CompareTo(phaseViewIndex);
            });
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
                phaseViews[i].SetActive(i == phaseIndex);
            }

            phaseViews[phaseIndex].ActivateAllBlocks();
        }
    }
}
