using UnityEngine;
using UnityEngine.UI;

public class CheckZoneHighlight : MonoBehaviour
{
    [SerializeField] private Image ringImage;
    [SerializeField] private Color inactiveColor = Color.white;
    [SerializeField] private Color activeColor = Color.green;

    public bool IsActive { get; private set; }

    private void Awake()
    {
        if (ringImage == null)
            ringImage = GetComponent<Image>();

        SetInactive();
    }

    public void SetActive()
    {
        IsActive = true;
        if (ringImage != null)
            ringImage.color = activeColor;
    }

    public void SetInactive()
    {
        IsActive = false;
        if (ringImage != null)
            ringImage.color = inactiveColor;
    }
}
