using System;
using UnityEngine;

public class Passengers : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Transform[] _waypoints;
    private int _currentIndex = 0;
    private bool _isMoving = false;
    private bool _destroyAtEnd = false;
    private Transform _waitSlot;

    public Action<Passengers> OnArrivedAtDestination;
    private void Awake()
    {
        // Sahnedeki PassengerManager'ı bul
        PassengerManager manager = FindObjectOfType<PassengerManager>();
        if (manager != null)
        {
            manager.RegisterPassenger(this);
        }
        else
        {
            Debug.LogWarning("[Passengers] PassengerManager bulunamadı!");
        }
    }

    

    public void SetWaitSlot(Transform slot)
    {
        _waitSlot = slot;
    }

    private void Update()
    {
        if (!_isMoving || _waypoints == null || _waypoints.Length == 0) return;

        
        if (_currentIndex >= _waypoints.Length)
        {
            // Yürüyüş bittiğinde slot varsa oraya git
            if (_waitSlot != null)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _waitSlot.position,
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, _waitSlot.position) < 0.05f)
                {
                    _isMoving = false;
                    OnArrivedAtDestination?.Invoke(this);

                    if (_destroyAtEnd)
                        Destroy(gameObject, 0.2f);
                }
            }
            return; 
        }

        Transform target = _waypoints[_currentIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            _currentIndex++;
        }
    }




    public void SetWaypoints(Transform[] waypoints)
    {
        _waypoints = waypoints;
        Debug.Log($"[Passengers] {_waypoints.Length} waypoint yüklendi.");
    }

    public void StartMoving()
    {
        if (_waypoints == null || _waypoints.Length == 0)
        {
            Debug.LogWarning("[Passengers] StartMoving çağrıldı ama waypoint yok.");
            return;
        }

        _currentIndex = 0;
        _isMoving = true;
    }

    public void SetDestroyAtEnd(bool value)
    {
        _destroyAtEnd = value;
    }
}
