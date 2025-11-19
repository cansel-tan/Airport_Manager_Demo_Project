using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PaintBoardUI : MonoBehaviour
{
    [System.Serializable]
    public class ColorOption
    {
        public Button button;
        public Color color = Color.white;
        public Image indicator;
        [Range(0.3f, 1f)] public float selectedScale = 0.7f;
    }

    [Header("Canvas & Alan")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform inputArea;

    [Header("UI")]
    [SerializeField] private RawImage boardRaw;
    [SerializeField] private TextMeshProUGUI percentText;
    [SerializeField] private Slider brushSizeSlider;

    [Header("Renk Seçenekleri")]
    [SerializeField] private List<ColorOption> colorOptions = new List<ColorOption>();

    [Header("3D Tahta (opsiyonel)")]
    [SerializeField] private Renderer boardRenderer;
    [SerializeField] private string colorPropName = "_Color";

    [Header("Kamera")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Transform paintCamTarget;
    [SerializeField] private float camMoveDuration = 0.8f;
    [SerializeField] private float camFov = 35f;

    [Header("Boyama Ayarları")]
    [SerializeField] private int texWidth = 1024;
    [SerializeField] private int texHeight = 1024;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private Color defaultPaintColor = Color.green;

    [Header("Mask (opsiyonel)")]
    [SerializeField] private Texture2D paintMask;
    [SerializeField] private float maskThreshold = 0.5f;
    [SerializeField] private bool autoOpenOnStart = true;
    [SerializeField] private bool moveCameraOnOpen = false;

    private Color32[] maskPixels;
    private bool useMask;
    private Texture2D paintTex;
    private Color paintColor;
    private bool[] paintedMask;
    private int paintedCount, totalPaintable;
    private bool paintingActive;

    void Awake()
    {
        if (!canvas) canvas = GetComponentInParent<Canvas>(true);
        if (!mainCam) mainCam = Camera.main;
        if (!inputArea && boardRaw) inputArea = boardRaw.rectTransform;

        
        paintTex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        Color[] fill = new Color[texWidth * texHeight];
        for (int i = 0; i < fill.Length; i++) fill[i] = startColor;
        paintTex.SetPixels(fill);
        paintTex.Apply();

        if (boardRaw)
        {
            boardRaw.texture = paintTex;
            boardRaw.raycastTarget = false; 
            boardRaw.transform.SetAsFirstSibling(); 
        }

        // Mask kontrolü
        useMask = (paintMask != null);
        if (useMask)
        {
            maskPixels = paintMask.GetPixels32();
            totalPaintable = 0;
            for (int i = 0; i < maskPixels.Length; i++)
            {
                float a = maskPixels[i].a / 255f;
                float l = (maskPixels[i].r + maskPixels[i].g + maskPixels[i].b) / (255f * 3f);
                if (Mathf.Max(a, l) > maskThreshold) totalPaintable++;
            }
        }
        else totalPaintable = texWidth * texHeight;

        paintedMask = new bool[texWidth * texHeight];
        paintColor = defaultPaintColor;

        // Renk butonlarını bağla
        for (int i = 0; i < colorOptions.Count; i++)
        {
            int idx = i;
            colorOptions[i].button.onClick.AddListener(() => SelectColor(idx));
        }
        if (colorOptions.Count > 0) SelectColor(0);

        UpdatePercent(0f);
    }

    void Start()
    {
        if (autoOpenOnStart)
            StartCoroutine(OpenRoutine());
    }

    private void SelectColor(int index)
    {
        if (index < 0 || index >= colorOptions.Count) return;
        paintColor = colorOptions[index].color;
        ApplyBoardMaterialColor(paintColor);

        for (int i = 0; i < colorOptions.Count; i++)
        {
            var opt = colorOptions[i];
            if (opt.indicator) opt.indicator.enabled = (i == index);
            var rt = opt.button.transform as RectTransform;
            rt.localScale = Vector3.one * (i == index ? opt.selectedScale : 0.5f);
        }
    }

    private void ApplyBoardMaterialColor(Color c)
    {
        if (boardRenderer && boardRenderer.material)
        {
            var mat = boardRenderer.material;
            if (mat.HasProperty(colorPropName)) mat.SetColor(colorPropName, c);
            else if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
            else if (mat.HasProperty("_TintColor")) mat.SetColor("_TintColor", c);
        }
    }

    public IEnumerator OpenRoutine()
    {
        if (paintingActive) yield break;
        paintingActive = true;

        gameObject.SetActive(true);
        yield return null;

        if (moveCameraOnOpen)
            yield return MoveCameraToTarget();
    }

    void Update()
    {
        if (!paintingActive) return;
        if (Input.GetMouseButton(0)) TryPaintAtPointer(Input.mousePosition);
    }

    private void TryPaintAtPointer(Vector2 screenPos)
    {
        if (!boardRaw || !inputArea) return;

        var cam = canvas && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        var checkRect = boardRaw.rectTransform;
        if (!RectTransformUtility.RectangleContainsScreenPoint(checkRect, screenPos, cam))
            return;

        // Ekran konumunu lokal koordinata çevir
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            boardRaw.rectTransform, screenPos, cam, out Vector2 localPoint))
            return;

        // Pivot farkını düzelt
        Rect r = boardRaw.rectTransform.rect;
        float u = Mathf.InverseLerp(r.xMin, r.xMax, localPoint.x);
        float v = Mathf.InverseLerp(r.yMin, r.yMax, localPoint.y);



        if (u < 0f || u > 1f || v < 0f || v > 1f) return;

        int px = Mathf.RoundToInt(u * (texWidth - 1));
        int py = Mathf.RoundToInt(v * (texHeight - 1));

        int radius = Mathf.RoundToInt(Mathf.Lerp(8f, 48f, brushSizeSlider ? brushSizeSlider.value : 0.5f));
        DrawCircle(px, py, radius, paintColor);
        paintTex.Apply();

        float percent = (paintedCount / (float)totalPaintable) * 100f;
        UpdatePercent(percent);
    }

    private void DrawCircle(int cx, int cy, int radius, Color col)
    {
        int r2 = radius * radius;
        int xmin = Mathf.Max(cx - radius, 0);
        int xmax = Mathf.Min(cx + radius, texWidth - 1);
        int ymin = Mathf.Max(cy - radius, 0);
        int ymax = Mathf.Min(cy + radius, texHeight - 1);

        for (int y = ymin; y <= ymax; y++)
        {
            int dy = y - cy; int dy2 = dy * dy;
            for (int x = xmin; x <= xmax; x++)
            {
                int dx = x - cx;
                if (dx * dx + dy2 > r2) continue;

                int idx = y * texWidth + x;
                if (!paintedMask[idx]) { paintedMask[idx] = true; paintedCount++; }
                paintTex.SetPixel(x, y, col);
            }
        }
    }

    private void UpdatePercent(float p)
    {
        if (percentText)
            percentText.text = $"{Mathf.Clamp(Mathf.RoundToInt(p), 0, 100)}%";
    }

    private IEnumerator MoveCameraToTarget()
    {
        if (!mainCam || !paintCamTarget) yield break;
        yield break; 
    }
}
