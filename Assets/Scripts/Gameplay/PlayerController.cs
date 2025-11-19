using UnityEngine;
using Controls;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Joystick moveStick;

    [Header("References")]
    [SerializeField] private Transform cameraTransform; 

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 4f;

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (moveStick == null) return;

        Vector2 axis = moveStick.Axis;
        Vector3 inputDir = new Vector3(axis.x, 0f, axis.y);

        // Kamera yönüne göre dönüş
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * inputDir.z) + (camRight * inputDir.x);

        // Hareket
        _controller.Move(moveDir * moveSpeed * Time.deltaTime);

        // Dönme
        if (moveDir.sqrMagnitude > 0.001f)
            transform.forward = moveDir;
    }
}
