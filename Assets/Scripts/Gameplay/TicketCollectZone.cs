using UnityEngine;
using UnityEngine.UI;

public class TicketCollectZone : MonoBehaviour
{
    [SerializeField] private PassengerBoardingManager boardingManager;
    [SerializeField] private Image ringImage;
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private Color activeColor = Color.green;

    private bool used = false;

    private void Start()
    {
        if (ringImage != null)
            ringImage.color = inactiveColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (used) return;
        Debug.Log("Player zone'a GİRDİ2");

        used = true;

        if (ringImage != null)
            ringImage.color = activeColor;

        if (boardingManager != null)
            boardingManager.StartBoarding();
        else
            Debug.LogWarning("[TicketZone] BoardingManager atanmadı!");
    }
}
