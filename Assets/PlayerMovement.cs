using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float crouchTransitionSpeedMultiplier = 8f;

    [SerializeField] private float jumpForce = 5f;
    private float currentMoveSpeed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchMoveSpeed = 2f;
    [SerializeField] private LayerMask groundLayer;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    private bool isGrounded;
    private bool isJumpReady;
    private float jumpCooldown = 1f;
    private float jumpCooldownTimer;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        crouchAction = InputSystem.actions.FindAction("Crouch");
    }

    private void Update()
    {
        // Handle jump
        if (jumpAction.WasPressedThisFrame() && isGrounded && isJumpReady)
        {
            Jump();
        }

        CheckGrounded();

        if (!isJumpReady && isGrounded)
        {
            jumpCooldownTimer += Time.deltaTime;
            if (jumpCooldownTimer >= jumpCooldown)
            {
                isJumpReady = true;
                jumpCooldownTimer = 0f;
            }
        }

        // Handle movement
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Calculate movement direction based on camera's orientation
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Ensure that movement is only on the horizontal plane (y = 0)
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize to ensure consistent speed
        cameraForward.Normalize();
        cameraRight.Normalize();

        if (crouchAction.IsPressed())
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, crouchMoveSpeed, Time.deltaTime * crouchTransitionSpeedMultiplier);
            Vector3 currentScale = transform.localScale;
            float newYScale = Mathf.Lerp(currentScale.y, 0.5f, Time.deltaTime * crouchTransitionSpeedMultiplier);
            transform.localScale = new Vector3(currentScale.x, newYScale, currentScale.z);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, moveSpeed, Time.deltaTime * crouchTransitionSpeedMultiplier);
            Vector3 currentScale = transform.localScale;
            float newYScale = Mathf.Lerp(currentScale.y, 1f, Time.deltaTime * crouchTransitionSpeedMultiplier);
            transform.localScale = new Vector3(currentScale.x, newYScale, currentScale.z);
        }

        // Combine input with camera's orientation
        Vector3 moveDirection = (cameraRight * moveInput.x + cameraForward * moveInput.y) * currentMoveSpeed;

        // Preserve vertical velocity
        Vector3 moveVelocity = new Vector3(moveDirection.x, playerRigidbody.linearVelocity.y, moveDirection.z);

        // Apply velocity to the Rigidbody
        playerRigidbody.linearVelocity = moveVelocity;
    }

    private void FixedUpdate()
    {

    }

    private void CheckGrounded()
    {
        // Position the sphere at the player's feet
        Vector3 spherePosition = transform.position + Vector3.down * 1f;
        isGrounded = Physics.CheckSphere(spherePosition, 0.1f, groundLayer);
    }

    private void OnDrawGizmos()
    {
        Vector3 spherePosition = transform.position + Vector3.down * 1f;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(spherePosition, 0.1f);
    }

    private void Jump()
    {
        isGrounded = false;
        isJumpReady = false;
        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, jumpForce, playerRigidbody.linearVelocity.z);
    }
}
