using UnityEngine;

public class DirectionArrowUI : MonoBehaviour
{
    public Transform player;   
    public Transform target;   

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (player == null || target == null || _rect == null)
            return;

        Vector3 dir = target.position - player.position;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        
        _rect.localEulerAngles = new Vector3(0f, 0f, -angle);
    }
}
