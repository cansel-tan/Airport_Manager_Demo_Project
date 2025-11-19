using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TruckDriver : MonoBehaviour
{
    [Header("Path")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Settings")]
    public float moveSpeed = 4f;
    public float waitAtEnd = 1.5f;

    [Header("Luggage")]
    public Transform luggageRoot;   

    public void DriveOnce()
    {
        StartCoroutine(DriveRoutine());
    }

    private IEnumerator DriveRoutine()
    {
        Debug.Log("[TruckDriver] Start -> End");

        // Start noktasından End’e
        yield return MoveBetween(startPoint.position, endPoint.position);

        // Biraz bekle
        yield return new WaitForSeconds(waitAtEnd);

        // Kamyondaki bavulları boşalt
        if (luggageRoot != null)
        {
            List<Transform> toDestroy = new List<Transform>();
            foreach (Transform child in luggageRoot)
                toDestroy.Add(child);

            foreach (var t in toDestroy)
                Destroy(t.gameObject);
        }

        Debug.Log("[TruckDriver] End -> Start");

        // Boş kamyonu geri getir
        yield return MoveBetween(endPoint.position, startPoint.position);

        Debug.Log("[TruckDriver] yolculuk bitti");
    }

    private IEnumerator MoveBetween(Vector3 from, Vector3 to)
    {
        transform.position = from;

        while (Vector3.Distance(transform.position, to) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                to,
                moveSpeed * Time.deltaTime);

            yield return null;
        }
    }
}
