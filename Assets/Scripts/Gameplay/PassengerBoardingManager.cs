using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PassengerBoardingManager : MonoBehaviour
{
    [Header("Yolcular (bekleme alanındaki)")]
    [SerializeField] private List<Transform> passengers = new List<Transform>();

    [Header("Zemin Referansı")]
    [SerializeField] private Transform groundRef;

    [Header("Path Noktaları")]
    [SerializeField] private Transform xrayPoint;      
    [SerializeField] private Transform planePoint;     

    [Header("Hareket Ayarları")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float delayBetweenPassengers = 0.05f;

    [Header("Sayaç Ayarları")]
    [SerializeField] private int maxPassengerCount = 6;

    [Header("Upgrade Pad (opsiyonel)")]
    [SerializeField] private BoardMoneyZoneWithFill upperPaintPad;

    public BoardMoneyZoneWithFill moneyZoneWithFill;

    [Header("Pad Görsel Kökü")]
    [SerializeField] private GameObject padVisualRoot;      
    [SerializeField] private Color inactiveColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private Color activeColor = Color.white;

    
    private Renderer[] meshRenderers;          
    private SpriteRenderer[] spriteRenderers;  
    private Graphic[] uiGraphics;             

    [Header("Para Sistemi")]
    [SerializeField] private MoneyCollectEffect moneyEffect;
    [SerializeField] private int moneyPerPassenger = 20;
    [SerializeField] private float moneySpawnOffsetY = 0.6f;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI boardedText;

    private int currentMoney;
    private int boardedCount;
    private bool boardingStarted;

    private void Awake()
    {
       
        if (passengers.Count == 0)
        {
            foreach (Transform child in transform)
                passengers.Add(child);
        }

        UpdateBoardedText();
        UpdateMoneyText();

        
        if (padVisualRoot == null)
        {
            var t = transform.Find("PadVisual");
            padVisualRoot = (t != null ? t.gameObject : gameObject);
        }

        
        meshRenderers = padVisualRoot.GetComponentsInChildren<Renderer>(true);
        spriteRenderers = padVisualRoot.GetComponentsInChildren<SpriteRenderer>(true);
        uiGraphics = padVisualRoot.GetComponentsInChildren<Graphic>(true);

        
        SetPadVisible(false);
    }

    public void RegisterPassenger(Transform p)
    {
        if (p == null) return;
        if (!passengers.Contains(p)) passengers.Add(p);
    }

    public void StartBoarding()
    {
        if (boardingStarted) return;
        if (xrayPoint == null || planePoint == null)
        {
            Debug.LogError("[Boarding] xrayPoint veya planePoint atanmadı!");
            return;
        }
        boardingStarted = true;
        StartCoroutine(BoardRoutine());
    }

    private IEnumerator BoardRoutine()
    {
        var toProcess = new List<Transform>();
        var seen = new HashSet<int>();
        foreach (var p in passengers)
        {
            if (p == null) continue;
            if (seen.Add(p.GetInstanceID())) toProcess.Add(p);
        }

        int processed = 0;
        foreach (var p in toProcess)
        {
            if (p == null) continue;
            if (processed >= maxPassengerCount) break;

            var rb = p.GetComponent<Rigidbody>();
            bool wasKinematic = rb != null && rb.isKinematic;
            if (rb != null) rb.isKinematic = true;

            var oldMover = p.GetComponent<Passengers>();
            if (oldMover != null) oldMover.enabled = false;

            yield return MoveToGrounded(p, xrayPoint.position);

            if (moneyEffect != null)
            {
                Vector3 spawnPos = p.position + Vector3.up * moneySpawnOffsetY;
                moneyEffect.PlayPickupEffect(spawnPos);
            }
            currentMoney += moneyPerPassenger;
            UpdateMoneyText();

            yield return MoveToGrounded(p, planePoint.position);

            Destroy(p.gameObject);

            boardedCount = Mathf.Min(boardedCount + 1, maxPassengerCount);
            UpdateBoardedText();

            passengers.Remove(p);
            processed++;
            yield return new WaitForSeconds(delayBetweenPassengers);

            if (rb != null) rb.isKinematic = wasKinematic;
        }

        passengers.Clear();
        boardingStarted = false;

        if (upperPaintPad != null && boardedCount >= maxPassengerCount)
        {
            upperPaintPad.UnlockPad();
        }
        Debug.Log("yolcular bitti. renk değişti 3");

        SetPadVisible(true);                   // pad’i aç
        if (moneyZoneWithFill != null)
            moneyZoneWithFill.ActivatePadVisual(true); 
        if (upperPaintPad != null && boardedCount >= maxPassengerCount)
            upperPaintPad.UnlockPad();
    }

    public void NotifyBoardingCompleted()
    {
        if (moneyZoneWithFill != null)
            moneyZoneWithFill.ActivatePadVisual(true);
    }

    private IEnumerator MoveToGrounded(Transform t, Vector3 targetPos)
    {
        float groundY = groundRef != null ? groundRef.position.y : t.position.y;
        targetPos.y = groundY;

        Vector3 flatDir = targetPos - t.position; flatDir.y = 0f;
        if (flatDir.sqrMagnitude > 0.001f)
            t.rotation = Quaternion.LookRotation(flatDir);

        while (true)
        {
            Vector3 current = t.position; current.y = groundY;
            Vector3 next = Vector3.MoveTowards(current, targetPos, moveSpeed * Time.deltaTime);
            next.y = groundY;
            t.position = next;

            if (Vector3.Distance(next, targetPos) <= 0.05f) break;
            yield return null;
        }
    }

    public void SetPadVisible(bool visible)
    {
        var activeCol = activeColor;
        var inactiveCol = inactiveColor;

      
        foreach (var r in meshRenderers)
        {
            if (!r) continue;
            var m = r.material;
            if (m != null)
            {
                if (m.HasProperty("_Color")) m.color = visible ? activeCol : inactiveCol;
                else if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", visible ? activeCol : inactiveCol);
            }
           
        }

        
        foreach (var s in spriteRenderers)
        {
            if (!s) continue;
            s.color = visible ? activeCol : inactiveCol;
            
        }

        
        foreach (var g in uiGraphics)
        {
            if (!g) continue;
           
            g.canvasRenderer.cullTransparentMesh = false;
            g.color = visible ? activeCol : inactiveCol;  
                                                          
        }
    }

    private void UpdateBoardedText()
    {
        if (boardedText != null)
            boardedText.text = $"{boardedCount}/{maxPassengerCount}";
    }

    private void UpdateMoneyText()
    {
        if (moneyText != null)
            moneyText.text = currentMoney.ToString();
    }

    
    public void ActivatePadVisual(bool active = true) => SetPadVisible(active);
}
