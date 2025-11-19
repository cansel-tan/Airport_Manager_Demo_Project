using UnityEngine;

public class AreaGizmoDrawer : MonoBehaviour
{
    [Header("Area Size (X = width, Y = depth)")]
    public Vector2 areaSize = new Vector2(4f, 4f);

    [Header("Corner Handles")]
    public float cornerRadius = 0.2f;

    public Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        
        Vector3 center = transform.position;

        
        Vector3 size3D = new Vector3(areaSize.x, 0f, areaSize.y);

       
        Gizmos.DrawWireCube(center, size3D);

        
        Vector3 half = size3D / 2f;

        Vector3[] corners =
        {
            center + new Vector3(-half.x, 0f, -half.z), 
            center + new Vector3( half.x, 0f, -half.z), 
            center + new Vector3( half.x, 0f,  half.z), 
            center + new Vector3(-half.x, 0f,  half.z), 
        };

        foreach (var c in corners)
        {
            Gizmos.DrawWireSphere(c, cornerRadius);
        }
    }
}
