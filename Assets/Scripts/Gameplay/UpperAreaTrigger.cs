using UnityEngine;

public class UpperAreaTrigger : MonoBehaviour
{
    public CameraAreaController cameraController;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (cameraController != null)
        {
            cameraController.SetUpperArea(true);
        }
    }
}
