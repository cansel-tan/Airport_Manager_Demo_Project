using UnityEngine;
using UnityEngine.EventSystems;

namespace Controls
{
    public class Joystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("UI References")]
        [SerializeField] private RectTransform stickArea;   
        [SerializeField] private RectTransform stickKnob;   

        [Header("Settings")]
        [SerializeField] private float stickRadius = 80f;   

        private Vector2 _axis;      
        private Canvas _canvas;

        public Vector2 Axis => _axis;
        public float Horizontal => _axis.x;
        public float Vertical => _axis.y;

        private void Awake()
        {
            if (stickArea == null)
                stickArea = GetComponent<RectTransform>();

            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (stickArea == null || stickKnob == null)
                return;

            Camera uiCam = null;

            if (_canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                uiCam = _canvas.worldCamera;

          
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                stickArea,
                eventData.position,
                uiCam,
                out Vector2 localPoint
            );

           
            Vector2 centerToPoint = localPoint;
        
            Vector2 normalized = Vector2.ClampMagnitude(
                centerToPoint / (stickArea.sizeDelta / 2f),
                1f
            );

            _axis = normalized;

          
            stickKnob.anchoredPosition = normalized * stickRadius;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _axis = Vector2.zero;
            if (stickKnob != null)
                stickKnob.anchoredPosition = Vector2.zero;
        }
    }
}
