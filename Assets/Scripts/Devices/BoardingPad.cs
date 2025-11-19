using UnityEngine;

public class BoardingPad : MonoBehaviour
{
    [Header("Görsel Objeler")]
    [SerializeField] private GameObject padVisual; 
    [SerializeField] private GameObject heartVisual;  

    [Header("UI Panel")]
    [SerializeField] private GameObject upgradePanel; 

    [Header("Collider (opsiyonel)")]
    [SerializeField] private Collider triggerCollider;

    private bool isActive = false;

    private void Awake()
    {
        if (triggerCollider == null)
            triggerCollider = GetComponent<Collider>();


        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    
    public void ActivatePad()
    {
        isActive = true;

        
    }

  

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;
        

        // Player pad'e bastı
        if (upgradePanel != null)
            upgradePanel.SetActive(true);
    }

   
    public void ClosePanel()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);
    }
}
