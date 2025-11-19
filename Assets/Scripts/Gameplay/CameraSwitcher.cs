using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Kamera Noktaları")]
    public Transform lowerAreaCamPoint;   
    public Transform upperAreaCamPoint;   

    [Header("Ayarlar")]
    public float moveSpeed = 5f;

    private Transform _currentTarget;

    void Start()
    {
        
        _currentTarget = lowerAreaCamPoint;
        transform.position = _currentTarget.position;
        transform.rotation = _currentTarget.rotation;
    }

    void LateUpdate()
    {
        if (_currentTarget == null) return;

        // Kamera pozisyon/rotasyonunu hedefe taşı
        transform.position = Vector3.Lerp(
            transform.position,
            _currentTarget.position,
            moveSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            _currentTarget.rotation,
            moveSpeed * Time.deltaTime);
    }

    public void SwitchToUpper()
    {
        _currentTarget = upperAreaCamPoint;
    }

    public void SwitchToLower()
    {
        _currentTarget = lowerAreaCamPoint;
    }
}
