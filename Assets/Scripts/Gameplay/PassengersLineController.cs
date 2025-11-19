using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengersLineController : MonoBehaviour
{
    [Header("Path Points")]
    public Transform[] pathPoints;             

    [Header("Settings")]
    public float startDelayBetweenPassengers = 0.4f;

    private readonly List<PassengerMover> _passengers = new List<PassengerMover>();

    public void SetPassengers(List<Transform> passengerTransforms)
    {
        _passengers.Clear();

        foreach (Transform t in passengerTransforms)
        {
            var mover = t.GetComponent<PassengerMover>();
            if (mover != null)
            {
                mover.SetPath(pathPoints);
                _passengers.Add(mover);
            }
        }
    }

    public void StartMoving()
    {
        if (pathPoints == null || pathPoints.Length == 0)
        {
            Debug.LogError("[PassengersLineController] pathPoints boş!");
            return;
        }

        StartCoroutine(StartMoveRoutine());
    }

    private IEnumerator StartMoveRoutine()
    {
        foreach (var p in _passengers)
        {
            if (p != null)
            {
                p.StartMoving();
                yield return new WaitForSeconds(startDelayBetweenPassengers);
            }
        }
    }
}
