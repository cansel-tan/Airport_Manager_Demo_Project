using UnityEngine;
using UnityEngine.UI;

public class BaggagePickUpZone : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerBaggageStack playerStack;
    [SerializeField] private PassengerManager passengerManager;

    [Header("Rings")]
    public Image firstRing;          
    public Image secondRing;         
    public Color inactiveColor = Color.white;
    public Color activeColor = Color.green;

    private bool _used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_used) return;
        _used = true;

        if (playerStack == null)
        {
            Debug.LogError("[PickUp] playerStack atanmadı!");
            return;
        }

        // 1️⃣ TÜM valizleri topla
        Baggage[] allBags = FindObjectsOfType<Baggage>();
        foreach (var bag in allBags)
        {
            if (!bag.collected)
                bag.Collect(playerStack);
        }

        // 2️⃣ Halkaları güncelle
        if (firstRing != null) firstRing.color = inactiveColor;
        if (secondRing != null) secondRing.color = activeColor;

        // 3️⃣ Yolcuları merdivene doğru yürüt
        if (passengerManager != null)
        {
            Debug.Log("[PickUp] Yolcular yürümeye başlıyor.");
            passengerManager.StartAllPassengers();
        }
        else
        {
            Debug.LogWarning("[PickUp] passengerManager atanmadı, yolcular yürümüyor.");
        }
    }
}
