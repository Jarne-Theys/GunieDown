using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Threading;

public class AIPlayerAgent : Agent
{
    public Rigidbody rb;
    
    private Vector3 finalMoveDirection;
    public float moveSpeed;

    public float raycastDistance;
    public int numWallRaycasts;

    public GameObject target;

    private float totalTime = 0f;
    public int episodeDuration = 60;

    public float fovAngle;
    public float visionRange;
    private Vector3 lastKnownPlayerLocation;

    public float turnSpeed = 180f; // Max degrees turned per fixed update
    bool playerVisible = false;
    private UpgradeManager upgradeManager;
    [Tooltip("Which UpgradeDefinition counts as the base weapon?")]
    [SerializeField] private UpgradeDefinition targetDefinition;
    private ProjectileComponentBase weapon;
    private InputActivationComponent input;
    private BulletTracker bulletTracker;

    private float yawInput;
    private float pitchInput;
    [SerializeField] private GameObject weaponGo;
    
    // --- Reward Shaping Parameter ---
    [Header("Reward Shaping")]
    [Tooltip("Maximum angle (degrees) off target the agent can fire without penalty.")]
    public float maxFiringAngleThreshold = 10.0f;
    [Tooltip("Penalty for firing when aim angle exceeds the threshold.")]
    public float poorAimPenalty = -0.02f;
    [Tooltip("Penalty for attempting to fire when weapon is not ready.")]
    public float fireWhenNotReadyPenalty = -0.005f;
    [Tooltip("Small cost for firing any shot (encourages ammo conservation). Set to 0 to disable.")]
    public float fireCost = -0.001f;

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = SpawnPositions.aiPlayerSpawnPosition;
        transform.rotation = Quaternion.Euler(SpawnPositions.aiPlayerSpawnRotation);

        target.transform.position = SpawnPositions.mockHumanPlayerSpawn;
        target.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        lastKnownPlayerLocation = Vector3.zero;
        playerVisible = false;

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        
        bulletTracker.ClearTrackedBulletList();

        totalTime = 0f;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeAcquired += HandleUpgradeAcquired;
        }
        else
        {
            Debug.LogWarning("UpgradeManager not found on Enable, subscription skipped.", this);
        }
    }
    
    protected override void OnDisable()
    {
        base.OnDisable();
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradeAcquired -= HandleUpgradeAcquired;
        }
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        upgradeManager = GetComponent<UpgradeManager>();
        bulletTracker = GetComponent<BulletTracker>();
    }

    // Add a reward from external scripts
    // currently only used by bullets (when they hit the agent)
    public void AddExternalReward(float reward)
    {
        AddReward(reward);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Transform Observations
        if (sensor == null)
        {
            throw new ArgumentException("VectorSensor is null");
        }
        
        sensor.AddObservation(transform.position);
        sensor.AddObservation(transform.forward);
        sensor.AddObservation(rb.linearVelocity / moveSpeed); // Observe normalized velocity

        // Raycast Observations
        // do 4 raycasts
        float angleStep = 360f / numWallRaycasts;
        for (int i = 0; i < numWallRaycasts; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            // Simplified direction calculation, test?:
            // Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;
            
            Vector3 rayFrom = transform.position - Vector3.up * 0.5f;
            RaycastHit hit;
            bool wallDetected = false;
            float distanceToHitNormalized = raycastDistance; // Default to max distance
            var terrainLayerMask = LayerMask.GetMask("terrainLayer");
            
            if (Physics.Raycast(rayFrom, rayDirection, out hit, raycastDistance))
            {
                // Consider layers or specific tags if more than just "Wall" exists
                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    wallDetected = true;
                    distanceToHitNormalized = hit.distance / raycastDistance;
                }
                // Optional: Observe other object types? (e.g., obstacles, cover)
                // sensor.AddObservation(hit.collider.CompareTag("Obstacle")); // Example
            }
            sensor.AddObservation(wallDetected); // Bool observation
            sensor.AddObservation(distanceToHitNormalized); // Normalized distance
        }


        // track x bullets
        int maxTrackedBullets = bulletTracker.maxTrackedBullets;
        for (int i = 0; i < maxTrackedBullets; i++)
        {
            if (i < bulletTracker.trackedBullets.Count)
            {
                // (relative) position
                Vector3 bulletPosition = bulletTracker.trackedBullets[i].position;
                Vector3 aiPosition = transform.position;
                sensor.AddObservation(bulletPosition - aiPosition);

                // velocity
                Rigidbody bulletRb = bulletTracker.trackedBullets[i].GetComponent<Rigidbody>();
                Vector3 bulletVelocity = bulletRb.linearVelocity;
                sensor.AddObservation(bulletVelocity);
            }
            else
            {
                // (relative) position
                sensor.AddObservation(Vector3.zero);
                
                // velocity
                sensor.AddObservation(Vector3.zero);

            }
        }

        // Target Observations
        // Cache player visibility check result here for use in OnActionReceived reward shaping
        playerVisible = false; // Reset for this observation step
        Vector3 relativeLastKnown = Vector3.zero; // Default if player not visible
        float relativeLastKnownMagNormalized = 0f; // Default

        if (target != null) // Check if target exists
        {
             Collider targetCollider = target.GetComponentInChildren<Collider>(); // More robust way to find collider
             if (targetCollider != null)
             {
                Vector3 playerCenter = targetCollider.bounds.center;
                Vector3 directionToPlayer = (playerCenter - transform.position); // Keep non-normalized for distance
                float distanceToPlayer = directionToPlayer.magnitude;
                directionToPlayer.Normalize(); // Normalize now

                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                // Check FOV and Range
                if (angleToPlayer < fovAngle / 2f && distanceToPlayer <= visionRange)
                {
                    // Check Line of Sight (LOS) - start ray slightly above agent center to avoid ground clipping
                    Ray ray = new Ray(transform.position + Vector3.up * 0.5f, directionToPlayer);
                    RaycastHit hit;
                    // Raycast only against relevant layers (e.g., Player, Wall, Obstacles) for performance
                    // int targetCheckLayers = LayerMask.GetMask("Player", "Wall", "Default"); // Example
                    // if (Physics.Raycast(ray, out hit, visionRange, targetCheckLayers))
                    if (Physics.Raycast(ray, out hit, visionRange))
                    {
                        // Check if the first thing hit is the Player (or part of the player)
                        if (hit.transform.root == target.transform) // Or check hit.collider.CompareTag("Player")
                        {
                            lastKnownPlayerLocation = playerCenter; // Use center for LKP
                            playerVisible = true; // Set the flag
                        }
                         // else: something obstructs the view
                    }
                    // else: Raycast didn't hit anything within range 
                }
             }
             else 
             {
                 Debug.LogWarning("Target has no Collider component in its children.", target);
             } 
             
            // Calculate relative LKP regardless of current visibility
            if (lastKnownPlayerLocation != Vector3.zero) // Only calculate if we have a valid LKP
            {
                relativeLastKnown = lastKnownPlayerLocation - transform.position;
                // Normalize magnitude relative to vision range
                relativeLastKnownMagNormalized = Mathf.Clamp01(relativeLastKnown.magnitude / visionRange);
            }
        }
        else {
             // Handle case where target is null
             Debug.LogWarning("Target is null in CollectObservations.");
        }


        sensor.AddObservation(playerVisible); // Bool observation: Is the player currently visible?
        sensor.AddObservation(relativeLastKnown.normalized); // Direction to Last Known Position (zero vector if never seen)
        sensor.AddObservation(relativeLastKnownMagNormalized); // Normalized distance to LKP
        sensor.AddObservation(weapon.ReadyToFire);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement and rotation 
        float moveForwardBackward = actions.ContinuousActions[0];
        float moveLeftRight = actions.ContinuousActions[1];
        yawInput = actions.ContinuousActions[2];
        pitchInput = actions.ContinuousActions[3];

        // Calculate move direction based on agent's current orientation
        Vector3 moveDirectionForward = transform.forward * moveForwardBackward;
        Vector3 moveDirectionStrafe = transform.right * moveLeftRight;
        finalMoveDirection = (moveDirectionForward + moveDirectionStrafe).normalized * moveSpeed; // Normalize to prevent faster diagonal movement

        // Shooting
        bool wantsToFire = actions.DiscreteActions[0] == 1;

        if (wantsToFire)
        {
            bool canFire = weapon != null && weapon.ReadyToFire;
            bool targetAimGood = false;

            // Check aim quality only if the player is currently visible
             if (playerVisible && target != null)
             {
                 Collider targetCollider = target.GetComponentInChildren<Collider>();
                 if (targetCollider != null)
                 {
                    Vector3 currentTargetCenter = targetCollider.bounds.center;
                    Vector3 currentDirectionToPlayer = (currentTargetCenter - transform.position).normalized;
                    // Compare agent's forward direction with direction to target
                    float currentAngleToPlayer = Vector3.Angle(transform.forward, currentDirectionToPlayer);

                    targetAimGood = currentAngleToPlayer <= maxFiringAngleThreshold;
                 }
             }


            // Apply penalties based on checks
            if (!canFire)
            {
                // Penalize trying to fire when weapon isn't ready
                AddReward(fireWhenNotReadyPenalty);
            }
            // Only penalize poor aim if the player is visible (otherwise agent might be predicting/pre-firing)
            else if (playerVisible && !targetAimGood)
            {
                // Penalize firing when aim is poor and player is visible
                 AddReward(poorAimPenalty);
                 // Debug.Log($"AI fired with poor aim ({currentAngleToPlayer} degrees). Penalty applied.");
            }

            // If checks pass (or if firing blind), attempt to fire
            if (canFire && input != null) // Ensure input component exists
            {
                // Apply the general cost for firing a shot
                AddReward(fireCost);

                // Trigger the weapon
                input.TriggerAction();
            }
            else if (input == null)
            {
                 Debug.LogWarning("AI tried to fire, but InputActivationComponent is missing.", this);
            }
             // Note: Penalties for !canFire and poor aim are handled above.
        }

        // Optional: Small reward for facing the player when visible?
        // Be careful not to make it override hitting/dodging rewards.
        // if (playerVisible && targetAimGood) { AddReward(0.001f); }

         // Small penalty for existing to encourage ending the episode faster? Usually not needed.
         // AddReward(-0.0001f);
    }

    private void FixedUpdate()
    {
        totalTime += Time.fixedDeltaTime;
        if (totalTime > episodeDuration)
        {
            EndEpisode();
        }
     
        // Bullet tracking
        bulletTracker.ClearTrackedBulletList();
        bulletTracker.DetectBullets();
        
        Vector3 desiredVelocity = finalMoveDirection;
        desiredVelocity.y = rb.linearVelocity.y; // Keep the vertical (gravity) velocity untouched
        Vector3 velocityChange = desiredVelocity - rb.linearVelocity;
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        
        float yawDegrees = yawInput * turnSpeed * Time.fixedDeltaTime;
        float pitchDegrees = -pitchInput * turnSpeed * Time.fixedDeltaTime;

        transform.Rotate(0f, yawDegrees, 0f);
        weaponGo.transform.Rotate(pitchDegrees, 0f, 0f);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-1f);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.black);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            AddReward(-0.5f);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.yellow);
        }
    }
    
    private void HandleUpgradeAcquired(UpgradeDefinition upgradeDefinition, IUpgradeComponent upgradeComponentInterface)
    {
        if (upgradeDefinition != targetDefinition) 
            return;

        if (upgradeComponentInterface is ProjectileComponentBase projectileComponentBase)
        {
            weapon = projectileComponentBase;
        }
        
        if (upgradeComponentInterface is InputActivationComponent inputActivationComponent)
        {
            input = inputActivationComponent;
        }
    }
    
    void DrawWallDetectionLinesV3()
    {
        float angleStep = 360f / numWallRaycasts;
        for (int i = 0; i < numWallRaycasts; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;

            //Vector3 rayFrom = transform.position;
            Vector3 rayFrom = transform.position - Vector3.up * 0.5f;
            
            Gizmos.DrawRay(rayFrom, rayDirection * raycastDistance);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        DrawWallDetectionLinesV3();

        if (target == null) return;

        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center;
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                           Vector3.Distance(transform.position, target.transform.position) <= visionRange;

        if (playerInFOV)
        {
            Ray ray = new Ray(transform.position, directionToPlayer);
            //Debug.DrawRay(transform.position, directionToPlayer * visionRange, Color.blue);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(transform.position, hit.point);
                if (hit.collider.CompareTag("Player"))
                {
                    Gizmos.color = Color.green;
                    //Gizmos.DrawLine(transform.position, playerCenter);
                } 
                else
                {
                    Gizmos.color = Color.yellow;
                }
            }
        } 
        else
        {
            Gizmos.color = Color.red;
        }

        DrawVisionCone();

    }
    
    private void DrawVisionCone()
    {
        int segments = 10; // How smooth the cone should be
        float stepAngle = fovAngle / segments;
        Vector3 startDirection = Quaternion.Euler(0, -fovAngle / 2, 0) * transform.forward;

        Vector3 previousPoint = transform.position + startDirection * visionRange;
        Gizmos.DrawLine(previousPoint, transform.position);
        for (int i = 1; i <= segments; i++)
        {
            float angle = -fovAngle / 2 + stepAngle * i;
            Vector3 nextDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 nextPoint = transform.position + nextDirection * visionRange;

            Gizmos.DrawLine(transform.position, nextPoint);
            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }
    }
}