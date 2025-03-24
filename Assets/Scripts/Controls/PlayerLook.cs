using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*
public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform cameraTransform;

    public float maxDownAngle = 70f;
    public float maxUpAngle = -90f;
    private float cameraXRotation = 0f;

    public float mouseSensitivity = 100f;

    private InputAction mouseMovement;
    void Start()
    {
        mouseMovement = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        // Read mouse input
        Vector2 mouseInput = mouseMovement.ReadValue<Vector2>();

        // Rotate the player on the Y axis (horizontal rotation)
        Vector3 playerRotation = new Vector3(0f, mouseInput.x * mouseSensitivity/100, 0f);
        playerTransform.Rotate(playerRotation);

        // Adjust the camera's X rotation based on mouse input
        cameraXRotation -= mouseInput.y * mouseSensitivity / 100; // Subtract because we want to invert the Y input

        // Clamp the X rotation to the defined limits
        cameraXRotation = Mathf.Clamp(cameraXRotation, maxUpAngle, maxDownAngle);

        // Apply the clamped rotation to the camera
        cameraTransform.localEulerAngles = new Vector3(cameraXRotation, cameraTransform.localEulerAngles.y, 0f);
    }
}
*/

public class PlayerLook : MonoBehaviour
{
    [SerializeField] private Transform cameraHolder;

    public float maxDownAngle = 70f;
    public float maxUpAngle = -90f;
    private float cameraXRotation = 0f;

    public float mouseSensitivity = 100f;

    private InputAction mouseMovement;

    void Start()
    {
        mouseMovement = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        Vector2 mouseInput = mouseMovement.ReadValue<Vector2>();

        // Y-axis (horizontal) rotation — rotate the player itself
        transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity / 100);

        // X-axis (vertical) rotation — rotate the camera holder
        cameraXRotation -= mouseInput.y * mouseSensitivity / 100;
        cameraXRotation = Mathf.Clamp(cameraXRotation, maxUpAngle, maxDownAngle);

        cameraHolder.localEulerAngles = new Vector3(cameraXRotation, 0f, 0f);
    }
}
