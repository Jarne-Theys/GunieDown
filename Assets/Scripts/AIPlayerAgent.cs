using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Threading;
using TMPro;
using Random = UnityEngine.Random;

public class AIPlayerAgent : Agent
{
    public bool endEpisodes = true;
    private float[] wallDistances;
    
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

    public float turnSpeed = 180f;
    bool playerVisible = false;
    private UpgradeManager upgradeManager;
    [Tooltip("Which UpgradeDefinition counts as the base weapon?")]
    [SerializeField] private UpgradeDefinition targetDefinition;
    private ProjectileComponentBase weapon;
    private InputActivationComponent input;
    private BulletTracker bulletTracker;

    private int yawInput;
    private float pitchInput;
    [SerializeField] private GameObject weaponGo;
    
    [SerializeField] private GameObject mainCamera;
    public bool pitchCamera;

    private TMP_Text debugText;

    private float lastMouseX;
    private float lastMouseY;
    [SerializeField] private float mouseSens;
    
    [SerializeField] bool lockCursor = true;
    
    
    [Header("Reward Shaping")]
    [Tooltip("Maximum angle (degrees) off target the agent can fire without penalty.")]
    public float maxFiringAngleThreshold = 10.0f;

    public override void OnEpisodeBegin()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        target.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        target.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        
        // transform.position = SpawnPositions.aiPlayerSpawnPosition;
        // transform.rotation = Quaternion.Euler(SpawnPositions.aiPlayerSpawnRotation);
        // target.transform.position = SpawnPositions.mockHumanPlayerSpawn;

        int randomZCoord = Random.Range(5, 45);
        int randomZCoordTarget = Random.Range(55, 95);
        
        Vector3 randomCoord = new Vector3(50, 1, randomZCoord);
        Vector3 randomCoordTarget = new Vector3(50, 1, randomZCoordTarget);

        transform.position = randomCoord;
        target.transform.position = randomCoordTarget;

        lastKnownPlayerLocation = Vector3.zero;
        playerVisible = false;

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }
        
        bulletTracker.ClearTrackedBulletList();

        totalTime = 0f;

        lastMouseX = 0;
        lastMouseY = 0;
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
        if (lockCursor)
        {
            //Cursor.lockState = CursorLockMode.Confined;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

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
        wallDistances = new float[numWallRaycasts];
    }

    // Add a reward from external scripts
    public void AddExternalReward(float reward, string message = "")
    {
        AddReward(reward);
        //if (message != "") Debug.Log(message);
    }

    public void EndEpisodeExternal(string reason)
    {
        Debug.Log($"Ending episode because {reason}");
        if (endEpisodes) EndEpisode();
        else Debug.Log("Not ending episode, as endEpisode is set to false");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Transform Observations
        if (sensor == null)
        {
            throw new ArgumentException("VectorSensor is null");
        }
        
        //sensor.AddObservation(transform.position);
        //sensor.AddObservation(transform.forward);
        //sensor.AddObservation(rb.linearVelocity / moveSpeed); // Observe normalized velocity


        // Raycast Observations
        // do 4 raycasts
        float angleStep = 360f / numWallRaycasts;
        for (int i = 0; i < numWallRaycasts; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            
            Vector3 rayFrom = transform.position - Vector3.up * 0.5f;
            RaycastHit hit;
            bool wallDetected = false;
            float distanceToHitNormalized = raycastDistance;
            var terrainLayerMask = LayerMask.GetMask("terrainLayer");
            
            if (Physics.Raycast(rayFrom, rayDirection, out hit, raycastDistance))
            {
                // Consider layers or specific tags if more than just "Wall" exists
                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    wallDetected = true;
                    distanceToHitNormalized = hit.distance / raycastDistance;
                }
            }
            sensor.AddObservation(distanceToHitNormalized);
            wallDistances[i] = distanceToHitNormalized;
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
                sensor.AddObservation((bulletPosition - aiPosition).normalized);
            }
            else
            {
                // (relative) position
                sensor.AddObservation(Vector3.zero);
                
            }
        }

        // Target Observations
        playerVisible = false;
        Vector3 relativeLastKnown = Vector3.zero;
        float relativeLastKnownMagNormalized = 0f;

        if (target != null)
        {
             Collider targetCollider = target.GetComponentInChildren<Collider>(); // More robust way to find collider
             if (targetCollider != null)
             {
                Vector3 playerCenter = targetCollider.bounds.center;
                Vector3 directionToPlayer = (playerCenter - transform.position); // Keep non-normalized for distance
                float distanceToPlayer = directionToPlayer.magnitude;
                directionToPlayer.Normalize();

                float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

                // Check FOV  Range
                if (angleToPlayer < fovAngle / 2f && distanceToPlayer <= visionRange)
                {
                    Ray ray = new Ray(transform.position + Vector3.up * 0.5f, directionToPlayer);
                    RaycastHit hit;
                    
                    if (Physics.Raycast(ray, out hit, visionRange))
                    {
                        // Check if the first thing hit is the Player (or part of the player)
                        if (hit.transform.root == target.transform) 
                        {
                            lastKnownPlayerLocation = playerCenter;
                            playerVisible = true; 
                            Debug.DrawRay(ray.origin, ray.direction * distanceToPlayer, Color.magenta, 20f);

                        }
                    }
                }
             }
             else 
             {
                 Debug.LogWarning("Target has no Collider component in its children.", target);
             } 
             
             if (lastKnownPlayerLocation != Vector3.zero) 
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
        

        sensor.AddObservation(playerVisible);
        sensor.AddObservation(relativeLastKnown.normalized); 
        sensor.AddObservation(relativeLastKnownMagNormalized);
        sensor.AddObservation(weapon.ReadyToFire);
        
        var weaponAngleToTarget = Vector3.Angle(transform.forward, relativeLastKnown);
        //sensor.AddObservation(weaponGo.transform.localRotation);
        sensor.AddObservation(weaponAngleToTarget);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Movement
        int forwardAction = actions.DiscreteActions[0]; // 0 = no move, 1 = forward, 2 = backward
        int strafeAction = actions.DiscreteActions[1];  // 0 = no move, 1 = right, 2 = left

        float moveForwardBackward = 0f;
        float moveLeftRight = 0f;

        switch (forwardAction)
        {
            case 1:
                moveForwardBackward = 1f;
                break;
            case 2:
                moveForwardBackward = -1f;
                break;
        }

        switch (strafeAction)
        {
            case 1:
                moveLeftRight = 1f;
                break;
            case 2:
                moveLeftRight = -1f;
                break;
        }

        yawInput = actions.DiscreteActions[3];

        Vector3 moveDirectionForward = transform.forward * moveForwardBackward;
        Vector3 moveDirectionStrafe = transform.right * moveLeftRight;
        finalMoveDirection = (moveDirectionForward + moveDirectionStrafe).normalized * moveSpeed;
 
        
        // Shooting
        bool wantsToFire = actions.DiscreteActions[2] == 1;
        
        
        Collider targetCollider = target.GetComponentInChildren<Collider>();

        Vector3 currentTargetCenter = targetCollider.bounds.center;
        Vector3 currentDirectionToPlayer = (currentTargetCenter - transform.position).normalized;
        
        float currentAngleToPlayer = Vector3.Angle(transform.forward, currentDirectionToPlayer);


        if (wantsToFire)
        {
            bool canFire = weapon != null && weapon.ReadyToFire;
            
            if (canFire && input != null)
            {
                // Manually trigger the InputAction component, as the AI can't use InputActions
                input.TriggerAction();
            }
            else if (input == null)
            {
                 Debug.LogWarning("AI tried to fire, but InputActivationComponent is missing.", this);
            }
        }
        
        foreach (float wallDistance in wallDistances)
        {
            AddReward(-wallDistance * 0.0001f);
        }

        double lookReward = Math.Pow(currentAngleToPlayer / 180, 2);
        AddReward((float)lookReward * 0.01f);
        
        float maxRadius = bulletTracker.detectionRadius;
        float penaltyScale = 0.1f;

        for (int i = 0; i < bulletTracker.trackedBullets.Count; i++)
        {
            Vector3 bulletPos = bulletTracker.trackedBullets[i].position;
            float distanceToBullet = Vector3.Distance(transform.position, bulletPos);

            if (distanceToBullet < maxRadius)
            {
                float distanceFraction = (maxRadius - distanceToBullet) / maxRadius;
                AddReward(-penaltyScale * distanceFraction * distanceFraction);
            }
        }
    }
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
        var discreteActionsOut = actionsOut.DiscreteActions;
        
        if (Input.GetKey(KeyCode.W))
            discreteActionsOut[0] = 1; // forward
        else if (Input.GetKey(KeyCode.S))
            discreteActionsOut[0] = 2; // backward
        else
            discreteActionsOut[0] = 0; // no movement

        // Left/Right
        if (Input.GetKey(KeyCode.D))
            discreteActionsOut[1] = 1; // right
        else if (Input.GetKey(KeyCode.A))
            discreteActionsOut[1] = 2; // left
        else
            discreteActionsOut[1] = 0; // no movement
        
        int x = 0;

        if (Input.GetKey(KeyCode.LeftArrow))  x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) x =  1;


        discreteActionsOut[3] = x;

        


        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) // 0 is Left Mouse Button
        {
            discreteActionsOut[2] = 1;
        }
        else
        {
            discreteActionsOut[2] = 0;
        }


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

        transform.Rotate(0f, yawDegrees, 0f);
        
        float currentPitch = weaponGo.transform.localEulerAngles.x;
        if (currentPitch > 180f) currentPitch -= 360f;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-10f);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.black, 20f);
            if (endEpisodes) EndEpisode();
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            AddReward(-0.5f);
            Debug.DrawLine(collision.transform.position, collision.transform.position + (Vector3.up * 20f), Color.yellow, 20f);
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

            string[] layerNames = {"playerLayer", "terrainLayer"};
            LayerMask layerMask = LayerMask.GetMask(layerNames);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange, layerMask))
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