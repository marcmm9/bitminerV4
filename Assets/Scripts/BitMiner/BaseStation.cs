using System;
using System.Collections;
using UnityEngine;

namespace BitMiner
{
    public abstract class BaseStation : MonoBehaviour
    {
        [Header("Station Connection Points")]
        public Transform inputPoint;
        public Transform outputPoint;
        
        [Header("Next Hardware Station")]
        public BaseStation nextStation;

        [Header("Station Configuration")]
        [SerializeField] protected float processingTime = 2.0f;
        [SerializeField] protected int currentLevel = 1;
        [SerializeField] protected float conveyorSpeed = 5.0f;

        // Public getters for configuration and level management
        public float ProcessingTime => processingTime;
        public int CurrentLevel => currentLevel;
        public float ConveyorSpeed => conveyorSpeed;

        protected virtual void Start()
        {
            // Self-validation warning in Unity Editor
            if (inputPoint == null)
            {
                Debug.LogWarning($"[BaseStation] 'inputPoint' is not assigned on {gameObject.name}! Please assign it in the Inspector.", this);
            }
            if (outputPoint == null)
            {
                Debug.LogWarning($"[BaseStation] 'outputPoint' is not assigned on {gameObject.name}! Please assign it in the Inspector.", this);
            }
        }

        /// <summary>
        /// Receives a block from a previous station (or spawner), moves it to the input point,
        /// and triggers the concrete processing of the block.
        /// </summary>
        public virtual void ReceiveBlock(DataBlock block)
        {
            if (block == null) return;
            
            // Move block from its current location to the input point
            StartCoroutine(MoveBlockCoroutine(block, inputPoint.position, () =>
            {
                ProcessBlock(block);
            }));
        }

        /// <summary>
        /// Concrete processing logic implemented by the hardware stations (CPU, GPU, Network).
        /// </summary>
        public abstract void ProcessBlock(DataBlock block);

        /// <summary>
        /// Call this when the station is finished processing a block.
        /// It will move the block to the output point and forward it to the next station.
        /// </summary>
        protected virtual void FinishProcessing(DataBlock block)
        {
            if (block == null) return;

            StartCoroutine(MoveBlockCoroutine(block, outputPoint.position, () =>
            {
                if (nextStation != null)
                {
                    nextStation.ReceiveBlock(block);
                }
                else
                {
                    Debug.Log($"[BaseStation] {gameObject.name} finished processing but 'nextStation' is not configured.", this);
                }
            }));
        }

        /// <summary>
        /// Helper coroutine to smoothly move a block's Transform towards a target position.
        /// </summary>
        protected IEnumerator MoveBlockCoroutine(DataBlock block, Vector3 targetPosition, Action onArrival)
        {
            while (block != null && Vector3.Distance(block.transform.position, targetPosition) > 0.01f)
            {
                block.transform.position = Vector3.MoveTowards(block.transform.position, targetPosition, conveyorSpeed * Time.deltaTime);
                yield return null;
            }

            if (block != null)
            {
                block.transform.position = targetPosition;
                onArrival?.Invoke();
            }
        }

        /// <summary>
        /// Abstract method to handle upgrades for this specific station.
        /// </summary>
        public abstract void UpgradeStation();
    }
}
