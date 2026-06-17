using System.Collections;
using UnityEngine;

namespace BitMiner
{
    public class StorageStation : BaseStation
    {
        [Header("Storage Specifics")]
        [SerializeField] private GameObject dataBlockPrefab;
        [SerializeField] private float baseSpawnInterval = 4.0f;
        [SerializeField] private float baseDataSize = 1.0f;

        private float spawnInterval;
        private float baseProcessingTime;

        protected override void Start()
        {
            base.Start();
            
            spawnInterval = baseSpawnInterval;
            baseProcessingTime = processingTime;
            
            // Recalculate stats based on starting level
            RecalculateStats();

            // Start spawning blocks
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            // Wait a moment at start
            yield return new WaitForSeconds(1.0f);

            while (true)
            {
                SpawnAndRegisterBlock();
                yield return new WaitForSeconds(spawnInterval);
            }
        }

        private void SpawnAndRegisterBlock()
        {
            if (dataBlockPrefab == null)
            {
                Debug.LogError($"[StorageStation] 'dataBlockPrefab' is null on {gameObject.name}! Please assign a DataBlock prefab.", this);
                return;
            }

            if (inputPoint == null) return;

            // Spawn block at input point
            GameObject spawnedObj = Instantiate(dataBlockPrefab, inputPoint.position, Quaternion.identity);
            DataBlock block = spawnedObj.GetComponent<DataBlock>();
            
            if (block != null)
            {
                // Set initial state
                block.SetState(BlockState.Raw);
                
                // RAM capacity upgrade: higher level increases data size
                float scalingFactor = 1.0f + (currentLevel - 1) * 0.25f;
                block.SetDataSize(baseDataSize * scalingFactor);

                // Start the station conveyor flow
                ProcessBlock(block);
            }
            else
            {
                Debug.LogWarning("[StorageStation] Spawned object does not have a DataBlock component!", this);
            }
        }

        public override void ProcessBlock(DataBlock block)
        {
            // Simulate reading from disk / loading into RAM
            StartCoroutine(ProcessStorageRead(block));
        }

        private IEnumerator ProcessStorageRead(DataBlock block)
        {
            // Wait for SSD read/processing time
            yield return new WaitForSeconds(processingTime);
            
            // Pass to output point and then next station
            FinishProcessing(block);
        }

        public override void UpgradeStation()
        {
            currentLevel++;
            RecalculateStats();
            Debug.Log($"[StorageStation] Upgraded to Level {currentLevel}! Spawn Interval: {spawnInterval:F2}s, Block Data Size Multiplier: {1.0f + (currentLevel - 1) * 0.25f:F2}x", this);
        }

        private void RecalculateStats()
        {
            // Higher level reduces spawn interval (faster SSD speed)
            spawnInterval = baseSpawnInterval / (1.0f + (currentLevel - 1) * 0.2f);
            
            // Higher level reduces disk read/transfer time
            processingTime = baseProcessingTime / (1.0f + (currentLevel - 1) * 0.15f);
        }
    }
}
