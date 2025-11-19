using System.Collections;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    [Header("Sequence Points")]
    [SerializeField] private Transform stairsViewPoint;     
    [SerializeField] private Transform upperAreaViewPoint;   

    [Header("Timings")]
    [SerializeField] private float segmentDuration = 1.2f;   
    [SerializeField] private float pauseBetween = 0.3f;      

    [Header("Optional")]
    [SerializeField] private PlayerController player_controller;        

    private bool isPlaying;

    public void PlayUpperAreaReveal()
    {
        if (!isPlaying)
            StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        isPlaying = true;

       
        if (player_controller != null)
            player_controller.enabled = false;

        
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        // Merdivenlere git
        if (stairsViewPoint != null)
            yield return MoveTo(stairsViewPoint.position, stairsViewPoint.rotation, segmentDuration);

        yield return new WaitForSeconds(pauseBetween);

        // Üst alana git
        if (upperAreaViewPoint != null)
            yield return MoveTo(upperAreaViewPoint.position, upperAreaViewPoint.rotation, segmentDuration);

        yield return new WaitForSeconds(pauseBetween);

        // Eski yerine dön
        yield return MoveTo(startPos, startRot, segmentDuration);

        // Player hareketini geri aç
        if (player_controller != null)
            player_controller.enabled = true;

        isPlaying = false;
    }

    private IEnumerator MoveTo(Vector3 targetPos, Quaternion targetRot, float duration)
    {
        Vector3 initialPos = transform.position;
        Quaternion initialRot = transform.rotation;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float s = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(initialPos, targetPos, s);
            transform.rotation = Quaternion.Slerp(initialRot, targetRot, s);

            yield return null;
        }
    }
}
