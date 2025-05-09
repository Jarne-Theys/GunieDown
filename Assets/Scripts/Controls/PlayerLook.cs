using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerLook : MonoBehaviour
{
    [Tooltip("Choose whether or not the script should also move the 'main camera'")]
    [SerializeField]
    private bool isOnMainPlayer;
    
    [SerializeField] 
    private Transform weaponTransform;

    public float maxDownAngle = 90f;
    public float maxUpAngle = -90f;
    private float cameraXRotation = 0f;

    public float mouseSensitivity = 100f;

    private InputAction mouseMovement;

    [SerializeField] private Camera mainCamera;

    void Start()
    {
        mouseMovement = InputSystem.actions.FindAction("Look");
    }

    void Update()
    {
        Vector2 mouseInput = mouseMovement.ReadValue<Vector2>();

        // Y-axis (horizontal) rotation - rotate the player itself
        transform.Rotate(Vector3.up * mouseInput.x * mouseSensitivity / 100);

        // X-axis (vertical) rotation - rotate the camera holder
        cameraXRotation -= mouseInput.y * mouseSensitivity / 100;
        cameraXRotation = Mathf.Clamp(cameraXRotation, maxUpAngle, maxDownAngle);

        if (weaponTransform != null) weaponTransform.localEulerAngles = new Vector3(cameraXRotation, 0f, 0f);
        if (mainCamera != null) mainCamera.transform.localEulerAngles = new Vector3(cameraXRotation, 0f, 0f);
    }
}
