using System.Collections;
using UnityEngine;

namespace BitMiner
{
    public class NetworkStation : BaseStation
    {
        [Header("Network Specifics")]
        [SerializeField] private float baseProcessingTime = 4.0f;
        [SerializeField] private double coinsPerDataSize = 10.0;
        [SerializeField] private double upgradeCurrencyPerDataSize = 1.0;

        protected override void Start()
        {
            base.Start();
            processingTime = baseProcessingTime;
            RecalculateStats();
        }

        public override void ProcessBlock(DataBlock block)
        {
            if (block == null) return;
            StartCoroutine(UploadBlockCoroutine(block));
        }

        private IEnumerator UploadBlockCoroutine(DataBlock block)
        {
            // Simulate network latency during upload
            yield return new WaitForSeconds(processingTime);

            if (block != null)
            {
                // Mark state as Uploaded
                block.SetState(BlockState.Uploaded);

                // Calculate earnings based on the data block's size
                double coinEarnings = block.DataSize * coinsPerDataSize;
                double upgradeCurrencyEarnings = block.DataSize * upgradeCurrencyPerDataSize;

                // Add funds to GameManager singleton
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddCoins(coinEarnings);
                    GameManager.Instance.AddUpgradeCurrency(upgradeCurrencyEarnings);
                    Debug.Log($"[NetworkStation] Block Uploaded! Earned +{coinEarnings:F1} Coins and +{upgradeCurrencyEarnings:F1} Upgrade Currency.");
                }
                else
                {
                    Debug.LogWarning("[NetworkStation] GameManager.Instance not found! Coins could not be awarded.");
                }

                // Since Network is the final station, destroy the block GameObject
                Destroy(block.gameObject);
            }
        }

        public override void UpgradeStation()
        {
            currentLevel++;
            RecalculateStats();
            
            string connectionType = GetConnectionTypeName();
            Debug.Log($"[NetworkStation] Upgraded connection to: {connectionType} (Level {currentLevel})! Upload Latency: {processingTime:F3}s", this);
        }

        private void RecalculateStats()
        {
            // Halves upload latency with each level (Modem -> ISDN -> DSL -> Cable -> Fiber)
            processingTime = baseProcessingTime / Mathf.Pow(2, currentLevel - 1);
            
            // If latency is extremely low, make it zero (instant upload)
            if (processingTime < 0.05f)
            {
                processingTime = 0.0f;
            }
        }

        private string GetConnectionTypeName()
        {
            switch (currentLevel)
            {
                case 1: return "56k Modem";
                case 2: return "ISDN Dual-Channel";
                case 3: return "DSL 16K";
                case 4: return "VDSL 100";
                case 5: return "Cable Gigabit";
                default: return "Fiber-Optics (FTTH)";
            }
        }
    }
}
