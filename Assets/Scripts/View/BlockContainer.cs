using System;
using UnityEngine;

namespace Erogemy.BlockBreaker.View
{
    public class BlockContainer : MonoBehaviour
    {
        [SerializeField] Block[] blocks = Array.Empty<Block>();

        public int Count => blocks.Length;

        void Awake()
        {
            blocks = GetComponentsInChildren<Block>();
        }

        public void SetActiveAll(bool isActive)
        {
            foreach (var block in blocks)
            {
                block.SetActive(isActive);
            }
        }
    }
}
