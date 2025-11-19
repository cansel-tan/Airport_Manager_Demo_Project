using UnityEngine;
using Controls;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Joystick moveStick;

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

        Vector3 move = inputDir.normalized * moveSpeed;
        _controller.Move(move * Time.deltaTime);


        if (inputDir.sqrMagnitude > 0.001f)
            transform.forward = inputDir;
    }
}