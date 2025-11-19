using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour
{
    [Header("Waypoints Root")]
    [SerializeField] private Transform waypointsRoot;   

    private readonly List<Passengers> _passengers = new List<Passengers>();
    private Transform[] _waypoints;

    [Header("Wait Slots (Üst Kat Bekleme Noktaları)")]
    [SerializeField] private Transform[] waitSlots;
    private int _nextSlotIndex = 0;

    private void Awake()
    {
        if (waypointsRoot == null)
        {
            Debug.LogError("[PassengerManager] waypointsRoot atanmadı!");
            return;
        }

        // WayPoints altındaki tüm pointleri sırayla al
        int count = waypointsRoot.childCount;
        _waypoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            _waypoints[i] = waypointsRoot.GetChild(i);
        }

        Debug.Log($"[PassengerManager] {_waypoints.Length} waypoint yüklendi.");
    }

    public Transform[] GetWaypoints()
    {
        return _waypoints;
    }

    public void RegisterPassenger(Passengers p)
    {
        if (p == null) return;

        _passengers.Add(p);
        p.SetWaypoints(_waypoints);

        
        if (waitSlots != null && waitSlots.Length > 0)
        {
            Transform assignedSlot = waitSlots[_nextSlotIndex % waitSlots.Length];
            p.SetWaitSlot(assignedSlot);
            Debug.Log($"[PassengerManager] {p.name} -> {assignedSlot.name} slotuna atandı.");
            _nextSlotIndex++;
        }
    }


    public void StartAllPassengers()
    {
        Debug.Log($"[PassengerManager] {_passengers.Count} yolcu yürümeye başlayacak.");
        foreach (var p in _passengers)
        {
            if (p != null)
                p.StartMoving();
        }
    }
}
