using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BitMiner
{
    public class GpuStation : BaseStation
    {
        [Header("GPU Specifics")]
        [SerializeField] private float baseProcessingTime = 4.0f;
        
        [Tooltip("Maximum parallel block processing capacity at Level 1")]
        [SerializeField] private int baseMaxParallelBlocks = 1;

        private Queue<DataBlock> waitingQueue = new Queue<DataBlock>();
        private List<DataBlock> activeBlocks = new List<DataBlock>();
        private int maxParallelBlocks = 1;

        protected override void Start()
        {
            base.Start();
            processingTime = baseProcessingTime;
            RecalculateStats();
        }

        public override void ProcessBlock(DataBlock block)
        {
            if (block == null) return;

            // Add the block to the waiting queue when it arrives at the inputPoint
            waitingQueue.Enqueue(block);
        }

        private void Update()
        {
            // Process blocks in parallel up to the maxParallelBlocks limit (VRAM level)
            while (activeBlocks.Count < maxParallelBlocks && waitingQueue.Count > 0)
            {
                DataBlock nextBlock = waitingQueue.Dequeue();
                if (nextBlock != null)
                {
                    activeBlocks.Add(nextBlock);
                    StartCoroutine(ProcessGpuTask(nextBlock));
                }
            }
        }

        private IEnumerator ProcessGpuTask(DataBlock block)
        {
            // Simulate GPU resolving cryptographic hash puzzles
            yield return new WaitForSeconds(processingTime);

            if (block != null)
            {
                // Set state to Mined (which turns the block Gold)
                block.SetState(BlockState.Mined);

                activeBlocks.Remove(block);
                
                // Move block to outputPoint and forward it to nextStation (Network)
                FinishProcessing(block);
            }
            else
            {
                // Safety cleanup in case the block was destroyed mid-process
                activeBlocks.Remove(block);
            }
        }

        public override void UpgradeStation()
        {
            currentLevel++;
            RecalculateStats();
            Debug.Log($"[GpuStation] Upgraded to Level {currentLevel}! Parallel VRAM Capacity: {maxParallelBlocks} blocks, Processing Time: {processingTime:F2}s", this);
        }

        private void RecalculateStats()
        {
            // VRAM/Cooling upgrades: Increases the amount of blocks processed simultaneously
            maxParallelBlocks = baseMaxParallelBlocks + (currentLevel - 1);
            
            // Moderate processing speed increase (frequency upgrade)
            processingTime = baseProcessingTime / (1.0f + (currentLevel - 1) * 0.1f);
        }
    }
}
