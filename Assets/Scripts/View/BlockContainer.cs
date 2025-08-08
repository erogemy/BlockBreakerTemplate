using System;
using UnityEngine;

namespace Erogemy.BlockBreaker.View
{
    public class BlockContainer : MonoBehaviour
    {
        [SerializeField] Block[] blocks = Array.Empty<Block>();

        public Block[] Blocks => blocks;

        void Awake()
        {
            blocks = GetComponentsInChildren<Block>();
        }
    }
}
