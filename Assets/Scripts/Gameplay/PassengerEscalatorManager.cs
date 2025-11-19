using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerEscalatorManager : MonoBehaviour
{
    [Header("Yolcular (otomatik dolacak)")]
    [SerializeField] private List<Transform> passengers = new List<Transform>();

    [Header("Yol Noktaları (alt kattan üst kata)")]
    [SerializeField] private Transform[] pathPoints;

    [Header("Üst kattaki bekleme slotları")]
    [SerializeField] private Transform[] waitSlots;

    [Header("Hız & Zamanlama")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float startDelayBetweenPassengers = 0.4f;

    private bool _started;

    private void Awake()
    {
        // PassengerLine altındaki tüm yolcuları listele
        passengers.Clear();
        foreach (Transform child in transform)
        {
            passengers.Add(child);
        }
    }

    
    public void StartMovingPassengers()
    {
        if (_started) return;
        _started = true;
        StartCoroutine(MovePassengersRoutine());
    }

    private IEnumerator MovePassengersRoutine()
    {
        for (int i = 0; i < passengers.Count; i++)
        {
            Transform passenger = passengers[i];
            if (passenger == null) continue;

            // Her yolcuyu küçük aralıklarla sırayla yürüt
            StartCoroutine(MoveSinglePassenger(passenger, i));
            yield return new WaitForSeconds(startDelayBetweenPassengers);
        }
    }

    private IEnumerator MoveSinglePassenger(Transform passenger, int passengerIndex)
    {
        // Ortak path noktaları: alt kuyruktan, merdiven üstüne, üst koridora
        if (pathPoints != null && pathPoints.Length > 0)
        {
            foreach (Transform p in pathPoints)
            {
                if (p == null) continue;
                yield return MoveToPoint(passenger, p.position);
            }
        }

        // En son: üst kattaki bekleme slotuna git
        if (waitSlots != null && waitSlots.Length > 0)
        {
            int slotIndex = Mathf.Min(passengerIndex, waitSlots.Length - 1);
            Transform slot = waitSlots[slotIndex];
            yield return MoveToPoint(passenger, slot.position);
        }
    }

    private IEnumerator MoveToPoint(Transform t, Vector3 targetPos)
    {
      
        Vector3 flatTarget = targetPos;
        flatTarget.y = t.position.y;  

        while (Vector3.Distance(t.position, flatTarget) > 0.05f)
        {
            
            Vector3 dir = flatTarget - t.position;
            dir.y = 0f;

            
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                t.rotation = Quaternion.Slerp(t.rotation, targetRot, 10f * Time.deltaTime);
            }

            
            t.position = Vector3.MoveTowards(
                t.position,
                flatTarget,
                moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
