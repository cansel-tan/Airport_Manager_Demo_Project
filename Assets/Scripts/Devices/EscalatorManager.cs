using System.Collections.Generic;
using UnityEngine;

public class EscalatorManager : MonoBehaviour
{
    [Header("Step Settings")]
    public Transform stepPrefab;      
    public int stepCount = 20;
    public float stepSpacing = 0.3f;

    [Header("Path")]
    public Transform startPoint;     
    public Transform endPoint;       
    public Vector3 lateralOffset = Vector3.zero;

    [Header("Movement")]
    public float moveSpeed = 1.5f; 
    public bool reverse = false;      

    [Header("Riders / Tags")]
    public string playerTag = "Player";
    public string passengerTag = "Passenger";

    [Header("Player Control (opsiyonel)")]
    [Tooltip("Player'ın hareket scriptini buraya sürükle (örn. JoystickPlayer, PlayerController vb.). Boş bırakırsan merdiven sadece konumunu taşır, hareket scriptini kapatmaz.")]
    public MonoBehaviour playerMovementScript;

    private readonly List<Transform> steps = new List<Transform>();
    private readonly List<Transform> riders = new List<Transform>();

    private Vector3 direction;         
    private float pathLength;

    private Transform internalStart;
    private Transform internalEnd;

    private Vector3 lateralWorld;

    void Start()
    {
        if (stepPrefab == null)
        {
            Debug.LogError("EscalatorManager: Step Prefab atanmadı!");
            return;
        }

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("EscalatorManager: StartPoint ve EndPoint atayın.");
            return;
        }

        // reverse true ise başlangıç ve bitişi değiştir
        internalStart = reverse ? endPoint : startPoint;
        internalEnd = reverse ? startPoint : endPoint;

        direction = (internalEnd.position - internalStart.position).normalized;
        pathLength = Vector3.Distance(internalStart.position, internalEnd.position);
        lateralWorld = transform.TransformVector(lateralOffset);

        if (pathLength <= 0.0001f)
        {
            Debug.LogError("EscalatorManager: StartPoint ile EndPoint aynı konumda olamaz.");
            return;
        }

       
        steps.Clear();
        for (int i = 0; i < stepCount; i++)
        {
            Transform step = Instantiate(stepPrefab, transform);
            step.rotation = transform.rotation;

            Vector3 pos = internalStart.position + direction * (i * stepSpacing) + lateralWorld;
            step.position = pos;

            steps.Add(step);
        }
    }

    void Update()
    {
        if (steps.Count == 0) return;

        float move = moveSpeed * Time.deltaTime;

        // 1) Basamakları hareket ettir
        foreach (var step in steps)
        {
            step.position += direction * move;

            float along = Vector3.Dot(step.position - internalStart.position, direction);

            
            if (along > pathLength)
            {
                float overflow = along - pathLength;
                step.position = internalStart.position + direction * overflow + lateralWorld;
            }
            else if (along < 0f)
            {
                float underflow = -along;
                step.position = internalEnd.position - direction * underflow + lateralWorld;
            }
        }

        
        for (int i = riders.Count - 1; i >= 0; i--)
        {
            Transform r = riders[i];
            if (r == null)
            {
                riders.RemoveAt(i);
                continue;
            }

            r.position += direction * move;

            float along = Vector3.Dot(r.position - internalStart.position, direction);

            
            if (along >= pathLength)
            {
                r.position = internalEnd.position + lateralWorld;

                
                var passenger = r.GetComponent<Passengers>();
                if (passenger != null)
                    passenger.enabled = true;

               
                if (playerMovementScript != null && r.CompareTag(playerTag))
                    playerMovementScript.enabled = true;

                riders.RemoveAt(i);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag) && !other.CompareTag(passengerTag))
            return;

        Transform t = other.transform;

        if (!riders.Contains(t))
        {
            riders.Add(t);

           
            var passenger = t.GetComponent<Passengers>();
            if (passenger != null)
                passenger.enabled = false;

            if (playerMovementScript != null && t.CompareTag(playerTag))
                playerMovementScript.enabled = false;

            // Ray üstüne hizala
            Vector3 projected =
                internalStart.position +
                direction * Vector3.Dot(t.position - internalStart.position, direction);

            t.position = projected + lateralWorld;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        if (!riders.Contains(t)) return;

        riders.Remove(t);

        var passenger = t.GetComponent<Passengers>();
        if (passenger != null)
            passenger.enabled = true;

        if (playerMovementScript != null && t.CompareTag(playerTag))
            playerMovementScript.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
            Gizmos.DrawSphere(startPoint.position, 0.05f);
            Gizmos.DrawSphere(endPoint.position, 0.05f);
        }
    }
}
