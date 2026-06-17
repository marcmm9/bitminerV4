using UnityEngine;

namespace BitMiner
{
    public enum BlockState
    {
        Raw,
        Decoded,
        Mined,
        Uploaded
    }

    public class DataBlock : MonoBehaviour
    {
        [Header("Data Block Settings")]
        [SerializeField] private BlockState state = BlockState.Raw;
        [SerializeField] private float dataSize = 1f;

        public BlockState State => state;
        public float DataSize => dataSize;

        private Renderer myRenderer;
        private SpriteRenderer mySpriteRenderer;

        private void Awake()
        {
            myRenderer = GetComponent<Renderer>();
            mySpriteRenderer = GetComponent<SpriteRenderer>();
            UpdateVisuals();
        }

        public void SetState(BlockState newState)
        {
            state = newState;
            UpdateVisuals();
        }

        public void SetDataSize(float size)
        {
            dataSize = Mathf.Max(0.1f, size);
        }

        public void UpdateVisuals()
        {
            Color color = Color.gray;

            switch (state)
            {
                case BlockState.Raw:
                    // Sleek Dark Gray
                    color = new Color(0.4f, 0.4f, 0.4f);
                    break;
                case BlockState.Decoded:
                    // Vibrant Neon Blue
                    color = new Color(0.0f, 0.6f, 1.0f);
                    break;
                case BlockState.Mined:
                    // Premium Gold
                    color = new Color(1.0f, 0.85f, 0.0f);
                    break;
                case BlockState.Uploaded:
                    // Soft Neon Green
                    color = new Color(0.2f, 0.9f, 0.3f);
                    break;
            }

            if (mySpriteRenderer != null)
            {
                mySpriteRenderer.color = color;
            }
            else if (myRenderer != null)
            {
                // Assign color directly to the material. 
                // In Unity URP/Built-in this creates an instance of the material.
                myRenderer.material.color = color;
            }
        }
    }
}
