using UnityEngine;

public class DirectionArrowFollower : MonoBehaviour
{
    [Header("References")]
    public Transform player;   
    public Transform target;   

    [Header("Placement")]
    public float height = 0.05f;        
    public float distanceInFront = 1.2f; 

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (player == null || target == null || _rect == null)
            return;

     
        Vector3 worldPos = player.position;
        worldPos.y += height;
        worldPos += player.forward * distanceInFront;

       
        _rect.position = worldPos;

        
        Vector3 toTarget = target.position - player.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;

            
            _rect.localEulerAngles = new Vector3(0f, 0f, -angle);
        }
    }
}
