using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaggageTruckLoader : MonoBehaviour
{
    [Header("Belt & Path")]
    public Transform beltRoot;
    public Transform xrayExitPoint;
    public Transform liftBottomPoint;
    public Transform liftTopPoint;

    [Header("Lift & Truck")]
    public Transform liftPlatform;
    public Transform truckPoint;
    public TruckDriver truckDriver;

    [Header("Stack Settings")]
    public float horizontalSpacing = 0.45f;

    [Tooltip("Kamyon kasasında zeminden ne kadar yüksekte dursun (base)")]
    public float verticalOffset = 0.02f;

    [Tooltip("Bir kolonda maksimum kaç valiz üst üste duracak")]
    public int stackPerColumn = 3;

    [Tooltip("İki valiz arasındaki dikey mesafe (üst üste koyma adımı)")]
    public float stackHeightStep = 0.18f;

    [Tooltip("True: X ekseninde diz, False: Z ekseninde diz")]
    public bool useXAxisForRow = true;

    [Tooltip("Varsayılan olarak valizi yatık koymak için önerilen local euler (sahnene göre değiştir)")]
    public Vector3 stackLocalRotationEuler = new Vector3(0f, 0f, 90f);

    [Header("Move Settings")]
    public float moveSpeed = 3f;
    public float delayBetweenBags = 0.15f;

    private bool _isRunning;

    public void StartLoading()
    {
        if (_isRunning) return;
        _isRunning = true;
        StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        List<Transform> bags = new List<Transform>();
        foreach (Transform child in beltRoot) bags.Add(child);

        int loadedCount = 0;

        for (int i = 0; i < bags.Count; i++)
        {
            Transform bag = bags[i];
            if (bag == null) continue;

            bag.SetParent(null, true);

            // XRAY çıkışına git
            yield return MoveTo(bag, xrayExitPoint.position);

            // Asansörün altına gel
            yield return MoveTo(bag, liftBottomPoint.position);

            // Bagajı asansöre ata
            bag.SetParent(liftPlatform, true);

            // 4Asansörü yukarı kaldır
            yield return MoveTo(liftPlatform, liftTopPoint.position);

            // 5️⃣ Kamyona yerleştir
            bag.SetParent(truckPoint, false);
            bag.localEulerAngles = stackLocalRotationEuler;

            int columnIndex = loadedCount / Mathf.Max(1, stackPerColumn);
            int stackIndex = loadedCount % Mathf.Max(1, stackPerColumn);
            Vector3 rowDir = useXAxisForRow ? Vector3.right : Vector3.forward;
            Vector3 localPos = rowDir * horizontalSpacing * columnIndex;
            localPos.y = verticalOffset + stackHeightStep * stackIndex;
            bag.localPosition = localPos;

            loadedCount++;

            // Asansörü geri indir
            yield return MoveTo(liftPlatform, liftBottomPoint.position);

            // Bagajlar arasında bekleme
            yield return new WaitForSeconds(delayBetweenBags);
        }

        _isRunning = false;
        if (truckDriver != null) truckDriver.DriveOnce();
        else Debug.LogWarning("[Loader] truckDriver = NULL !");
    }

    private IEnumerator MoveTo(Transform target, Vector3 worldPos, bool useLocal = false)
    {
        if (useLocal)
        {
            while (Vector3.Distance(target.localPosition, worldPos) > 0.01f)
            {
                target.localPosition = Vector3.MoveTowards(target.localPosition, worldPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
        else
        {
            while (Vector3.Distance(target.position, worldPos) > 0.01f)
            {
                target.position = Vector3.MoveTowards(target.position, worldPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

}