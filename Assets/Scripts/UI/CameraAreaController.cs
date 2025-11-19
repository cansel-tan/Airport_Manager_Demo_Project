using UnityEngine;

public class CameraAreaController : MonoBehaviour
{
    [Header("Targets")]
    public Transform lowerCamPoint;   
    public Transform upperCamPoint;   

    [Header("Settings")]
    public float followSpeed = 5f;
    public bool isUpperArea = false; 

    private Transform _targetPoint;

    private void Start()
    {
        
        _targetPoint = lowerCamPoint;

        
        if (_targetPoint != null)
        {
            transform.position = _targetPoint.position;
            transform.rotation = _targetPoint.rotation;
        }
    }

    private void LateUpdate()
    {
        if (_targetPoint == null) return;

        // Pozisyonu hedef noktaya taşı
        transform.position = Vector3.Lerp(
            transform.position,
            _targetPoint.position,
            followSpeed * Time.deltaTime);

        // Rotasyonu da hedef noktanın rotasyonuna çevir
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            _targetPoint.rotation,
            followSpeed * Time.deltaTime);
    }

    public void SetUpperArea(bool upper)
    {
        isUpperArea = upper;
        _targetPoint = isUpperArea ? upperCamPoint : lowerCamPoint;
    }
}
