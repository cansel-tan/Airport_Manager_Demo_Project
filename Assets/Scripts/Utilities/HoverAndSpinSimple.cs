using UnityEngine;

namespace Utilities
{
    public class HoverAndSpinSimple : MonoBehaviour
    {
        [SerializeField] private float spinSpeed = 90f;

        private Quaternion _startRot;

        private void Start()
        {
            _startRot = transform.rotation;
        }

        private void Update()
        {
            float angle = spinSpeed * Time.deltaTime;
            transform.rotation = _startRot * Quaternion.Euler(0f, Time.time * spinSpeed, 0f);
        }
    }
}
