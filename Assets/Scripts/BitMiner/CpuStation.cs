using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BitMiner
{
    public class CpuStation : BaseStation
    {
        [Header("CPU Specifics")]
        [SerializeField] private float baseProcessingTime = 3.0f;

        private Queue<DataBlock> processingQueue = new Queue<DataBlock>();
        private bool isProcessing = false;

        protected override void Start()
        {
            base.Start();
            processingTime = baseProcessingTime;
            RecalculateStats();
        }

        public override void ProcessBlock(DataBlock block)
        {
            if (block == null) return;

            // Enqueue the block when it reaches the inputPoint
            processingQueue.Enqueue(block);
        }

        private void Update()
        {
            // If the CPU is idle and there are blocks in the queue, process the next one
            if (!isProcessing && processingQueue.Count > 0)
            {
                DataBlock nextBlock = processingQueue.Dequeue();
                if (nextBlock != null)
                {
                    StartCoroutine(ProcessCpuTask(nextBlock));
                }
            }
        }

        private IEnumerator ProcessCpuTask(DataBlock block)
        {
            isProcessing = true;

            // Simulate the "Fetch-Decode-Execute" phase
            yield return new WaitForSeconds(processingTime);

            if (block != null)
            {
                // After processing, change state to Decoded (which turns it Blue)
                block.SetState(BlockState.Decoded);
                
                // Move block to outputPoint and forward it to nextStation (GPU)
                FinishProcessing(block);
            }

            isProcessing = false;
        }

        public override void UpgradeStation()
        {
            currentLevel++;
            RecalculateStats();
            Debug.Log($"[CpuStation] Upgraded to Level {currentLevel}! GHz (Speed) Multiplier: {1.0f + (currentLevel - 1) * 0.5f:F2}x, Processing Time: {processingTime:F2}s", this);
        }

        private void RecalculateStats()
        {
            // Higher level (higher GHz clock speed) increases processing speed and thus reduces processing time
            processingTime = baseProcessingTime / (1.0f + (currentLevel - 1) * 0.5f);
        }
    }
}
