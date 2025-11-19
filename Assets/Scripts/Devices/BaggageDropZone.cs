using UnityEngine;
using UnityEngine.UI;

public class BaggageDropZone : MonoBehaviour
{
    [Header("References")]
    public PlayerBaggageStack playerStack;

    [Header("Belt Settings")]
    public Transform beltRoot;          
    public float stackHeight = 0.3f;    

    [Header("Step Rings")]
    public Image secondRing;            
    public Image thirdRing;            
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;

    private bool _used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_used) return;

        Debug.Log("[DropToBelt] Trigger enter by Player");

        if (playerStack == null)
        {
            Debug.LogError("[DropToBelt] playerStack is NULL!");
            return;
        }

        if (beltRoot == null)
        {
            Debug.LogError("[DropToBelt] beltRoot is NULL!");
            return;
        }

        Debug.Log($"[DropToBelt] Stack count before drop = {playerStack.Count}");

        if (playerStack.Count <= 0)
        {
            Debug.Log("[DropToBelt] No baggage on player.");
            return;
        }

        // 1) Valizleri bandın başında kule şeklinde bırak
        playerStack.DropAllStacked(beltRoot, stackHeight);
        _used = true;

        // 2) Halka renkleri
        if (secondRing != null) secondRing.color = inactiveColor;
        if (thirdRing != null) thirdRing.color = activeColor;
    }
}
