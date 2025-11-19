using UnityEngine;
using UnityEngine.UI;

public class MoneyZoneWithFill : MonoBehaviour
{
  
    public int rewardAmount = 50;
    public bool oneTimeCollect = true;
    public GameObject visualsRoot;
    public GameObject nextAreaRoot;
    public GameObject nextCircleRoot;
    public GameObject nextArrowUpRoot;
    
    public MainCameraController mainCamera;
    [SerializeField] private CheckZoneHighlight nextCheckZone;

    public Image fillImage;          
    public float fillDuration = 1.5f;

    private bool playerInside;
    private bool collected;
    private float timer;

   

    private void Start()
    {
        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (!playerInside || collected)
            return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / fillDuration);

        if (fillImage != null)
            fillImage.fillAmount = t;

        if (t >= 1f)
        {
            GiveReward();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (oneTimeCollect && collected) return;

        Debug.Log("Player zone'a GİRDİ");
        playerInside = true;
        timer = 0f;

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (collected) return;

        Debug.Log("Player zone'dan ÇIKTI");
        playerInside = false;
        timer = 0f;

        if (fillImage != null)
            fillImage.fillAmount = 0f;
    }

    private void GiveReward()
    {
        collected = true;
        playerInside = false;

        PlayerWallet wallet = FindObjectOfType<PlayerWallet>();
        if (wallet != null)
        {
            wallet.AddMoney(rewardAmount);

            
            MoneyCollectEffect effect = wallet.GetComponent<MoneyCollectEffect>();
            if (effect != null)
            {
                
                effect.PlayPickupEffect(transform.position);
            }
        }

        if (visualsRoot != null)
            visualsRoot.SetActive(false);

        if (nextAreaRoot != null)
            nextAreaRoot.SetActive(true);

        if (nextCircleRoot != null)
            nextCircleRoot.SetActive(true);

        if (nextArrowUpRoot != null)
            nextArrowUpRoot.SetActive(true);

        if (nextCheckZone != null)
            nextCheckZone.SetActive();

        if (mainCamera != null)
            mainCamera.PlayUpperAreaReveal();
    }
}
