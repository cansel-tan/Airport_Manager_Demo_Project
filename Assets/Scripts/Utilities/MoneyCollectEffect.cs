using System.Collections;
using UnityEngine;

public class MoneyCollectEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform targetPoint;     
    [SerializeField] private GameObject moneyIconPrefab; 

    [Header("Animation Settings")]
    [SerializeField] private float flyDuration = 0.8f;
    [SerializeField] private float arcHeight = 0.5f;
    [SerializeField] private float spinSpeed = 360f;

    public void PlayPickupEffect(Vector3 startWorldPos)
    {
        if (targetPoint == null || moneyIconPrefab == null)
            return;

        StartCoroutine(FlyRoutine(startWorldPos));
    }

    private IEnumerator FlyRoutine(Vector3 startPos)
    {
        GameObject icon = Instantiate(moneyIconPrefab, startPos, Quaternion.identity);
        Transform iconT = icon.transform;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / flyDuration;
            float clamped = Mathf.Clamp01(t);

            
            Vector3 pos = Vector3.Lerp(startPos, targetPoint.position, clamped);

           
            pos.y += Mathf.Sin(clamped * Mathf.PI) * arcHeight;

            iconT.position = pos;

          
            iconT.Rotate(Vector3.up * spinSpeed * Time.deltaTime, Space.World);

            yield return null;
        }

        Destroy(icon);
    }
}
