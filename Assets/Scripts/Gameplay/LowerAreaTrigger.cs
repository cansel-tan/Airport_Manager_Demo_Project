using UnityEngine;

public class LowerAreaTrigger : MonoBehaviour
{
    public CameraAreaController cameraController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (cameraController != null)
        {
            cameraController.SetUpperArea(false);
        }
    }
}
