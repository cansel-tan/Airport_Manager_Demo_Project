using UnityEngine;

public class ObjectiveArrowController : MonoBehaviour
{
    
    [SerializeField] private Transform player;   
    [SerializeField] private Transform target;  

   
    [SerializeField] private float height = 0.2f;        
    [SerializeField] private float distanceInFront = 1.5f; 

    private void LateUpdate()
    {
        if (player == null || target == null) return;

        
        Vector3 basePos = player.position;
        basePos.y += height;
        basePos += player.forward * distanceInFront;
        transform.position = basePos;

       
        Vector3 toTarget = target.position - player.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(toTarget);
            transform.rotation = lookRot;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
