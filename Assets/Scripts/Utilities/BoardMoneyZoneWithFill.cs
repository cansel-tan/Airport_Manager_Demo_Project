using UnityEngine;
using UnityEngine.UI;

public class BoardMoneyZoneWithFill : MonoBehaviour
{
    [Header("Ödül")]
    public int rewardAmount = 50;
    public bool oneTimeCollect = true;

    [Header("Görsel Root'lar")]
    public GameObject visualsRoot;
    public GameObject nextAreaRoot;
    public MainCameraController mainCamera;
    public GameObject arrowMark;
    [SerializeField] private CheckZoneHighlight nextCheckZone;
    


    [Header("Fill Görseli")]
    public Image fillImage;
    public float fillDuration = 1.5f;

    [Header("Pad Görseli")]
    [SerializeField] private Image padImage;          
    public SpriteRenderer padRenderer;                
    [SerializeField] private GameObject padGroundRoot; 

    [Header("Renkler")]
    public Color inactiveColor = new Color(1, 1, 1, 0.12f);
    public Color activeLoadingColor = Color.green;   
    [SerializeField] private Color activeColorPad = Color.white;

    [Header("Durum")]
    [SerializeField] private bool startLocked = true;
    [SerializeField] private PassengerBoardingManager boardingManager;

    [Header("Board Ekranı")]
    [SerializeField] private GameObject upgradePanel;

    
    private bool isUnlocked;
    private bool playerInside;
    private bool collected;
    private float timer;

    private void Awake()
    {
        isUnlocked = !startLocked;

        if (padImage != null)
        {
            padImage.canvasRenderer.cullTransparentMesh = false;
            padImage.enabled = true;
        }

       
        if (padImage != null && fillImage != null && padImage == fillImage)
            Debug.LogWarning("[BoardMoneyZoneWithFill] padImage ve fillImage aynı objeye atanmış, Inspector'da ayır!");

        // pad her zaman sahnede 
        if (padGroundRoot != null)
            padGroundRoot.SetActive(true);

        // başlangıçta inaktif renkte
        SetPadColor(inactiveColor);

        if (fillImage != null) fillImage.fillAmount = 0f;
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isUnlocked) return;
        if (!playerInside || (collected && oneTimeCollect)) return;
        Debug.Log("yolcular bitti. renk değişti 1");

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fillDuration);

        if (padImage != null) padImage.color = activeColorPad;

        if (fillImage != null)
        {
            fillImage.color = activeLoadingColor;
            fillImage.fillAmount = t;
        }
        
        if (t >= 1f) GiveReward();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isUnlocked) return;
        Debug.Log("Player zone'a GİRDİ1");

        playerInside = true;
        timer = 0f;
        if (padImage != null) padImage.color = activeColorPad;

        

        if (fillImage != null) fillImage.fillAmount = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
        timer = 0f;
        if (fillImage != null) fillImage.fillAmount = 0f;

        
        SetPadColor(isUnlocked ? activeColorPad : inactiveColor);

        if (!collected && upgradePanel != null)
            upgradePanel.SetActive(false);
    }

    private void GiveReward()
    {
        collected = true;
        playerInside = false;

        var wallet = FindObjectOfType<PlayerWallet>();
        if (wallet != null)
        {
            wallet.AddMoney(rewardAmount);
            var effect = wallet.GetComponent<MoneyCollectEffect>();
            if (effect != null) effect.PlayPickupEffect(transform.position);
        }

        
        if (padGroundRoot != null) padGroundRoot.SetActive(true);
        if (padImage != null) padImage.color = activeColorPad;
        if (padRenderer != null) padRenderer.color = activeColorPad;

        if (upgradePanel != null) upgradePanel.SetActive(true);

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (nextAreaRoot != null) nextAreaRoot.SetActive(true);
        if (nextCheckZone != null) nextCheckZone.SetActive();
        if (mainCamera != null) mainCamera.PlayUpperAreaReveal();

        Debug.Log("[BoardMoneyZoneWithFill] PaintScene yükleniyor...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("PaintScene");
    }

    public void UnlockPad()
    {
        Debug.Log("yolcular bitti. renk değişti 3");
        if (padImage != null) padImage.color = activeColorPad;
        Debug.Log($"[UnlockPad] padImage rengi değişti → {padImage.color}");
        isUnlocked = true;
       
        if (fillImage != null) fillImage.fillAmount = 0f;
        if (arrowMark != null)  arrowMark.SetActive(true);
       
    }

    private void SetPadColor(Color c)
    {
        if (padImage != null) padImage.color = c;
        
    }

    public void ActivatePadVisual(bool active)
    {
        Debug.Log("yolcular bitti. renk değişti 2");

        if (fillImage != null) fillImage.fillAmount = 0f;

        if (padImage != null) padImage.color = activeColorPad;
        if (boardingManager != null) boardingManager.SetPadVisible(active);


    }
}
