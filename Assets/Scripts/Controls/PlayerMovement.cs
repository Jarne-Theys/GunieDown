using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Transform cameraHolder;

    [SerializeField] private float crouchTransitionSpeedMultiplier = 8f;

    [SerializeField] private float jumpForce = 5f;
    private float currentMoveSpeed;

    [HideInInspector]
    public float movementSpeedMultiplier = 1f;

    [SerializeField] private LayerMask groundLayer;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;

    private bool isGrounded;
    private bool isJumpReady;
    public float jumpCooldown = 0.1f;
    private float jumpCooldownTimer;

    private PlayerStats PlayerStats;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        crouchAction = InputSystem.actions.FindAction("Crouch");

        PlayerStats = GetComponent<PlayerStats>();
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
        Vector3 cameraForward = cameraHolder.forward;
        Vector3 cameraRight = cameraHolder.right;

        // Ensure that movement is only on the horizontal plane (y = 0)
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize to ensure consistent speed
        cameraForward.Normalize();
        cameraRight.Normalize();

        if (crouchAction.IsPressed())
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, (float)(PlayerStats.MovementSpeed / 2.5), Time.deltaTime * crouchTransitionSpeedMultiplier);
            Vector3 currentScale = transform.localScale;
            Vector3 currentPosition = transform.position;

            // Desired crouch height (assuming normal scale.y = 1)
            float targetHeight = 0.5f;
            float newYScale = Mathf.Lerp(currentScale.y, targetHeight, Time.deltaTime * crouchTransitionSpeedMultiplier);

            float scaleChange = newYScale - currentScale.y;

            transform.position += new Vector3(0, scaleChange * 0.5f, 0);

            transform.localScale = new Vector3(currentScale.x, newYScale, currentScale.z);
        }

        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, PlayerStats.MovementSpeed, Time.deltaTime * crouchTransitionSpeedMultiplier);
            Vector3 currentScale = transform.localScale;
            Vector3 currentPosition = transform.position;

            float targetHeight = 1f;
            float newYScale = Mathf.Lerp(currentScale.y, targetHeight, Time.deltaTime * crouchTransitionSpeedMultiplier);

            float scaleChange = newYScale - currentScale.y;

            transform.position += new Vector3(0, scaleChange * 0.5f, 0);
            transform.localScale = new Vector3(currentScale.x, newYScale, currentScale.z);
        }

        // Combine input with camera's orientation
        Vector3 moveDirection = (cameraRight * moveInput.x + cameraForward * moveInput.y) * currentMoveSpeed * movementSpeedMultiplier;

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
