using UnityEngine;

namespace Utilities
{
    public class ArrowHover : MonoBehaviour
    {
        [SerializeField] private float hoverHeight = 0.35f;
        [SerializeField] private float hoverSpeed = 2f;

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = transform.position;
        }

        private void Update()
        {
            float offset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

            
            transform.position = new Vector3(
                _startPos.x,
                _startPos.y + offset,
                _startPos.z
            );
        }
    }
}
