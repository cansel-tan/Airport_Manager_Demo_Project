using UnityEngine;

public class XrayDropZone : MonoBehaviour
{
    [SerializeField] private Transform xrayEntryPoint; 

    private void OnTriggerEnter(Collider other)
    {
        var stack = other.GetComponent<PlayerBaggageStack>();
        if (stack == null) return;

        // Valizleri banda bırak
        stack.DropAllStacked(xrayEntryPoint, 0.4f);
    }
}
