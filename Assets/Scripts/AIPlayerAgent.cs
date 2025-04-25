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
    private int episodeDuration = 60;

    public int bulletTrackCount;
    public float fovAngle;
    public float visionRange;
    private Vector3 lastKnownPlayerLocation;

    public float fireRate = 1f;

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

        bulletTracker.ClearTrackedBulletList();

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        foreach (GameObject bullet in bullets)
        {
            Destroy(bullet);
        }

        totalTime = 0f;
    }
    
    protected override void OnEnable()
    {
        base.OnEnable();
        upgradeManager.OnUpgradeAcquired += HandleUpgradeAcquired;
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

        // Raycast Observations
        // do 4 raycasts
        float angleStep = 360f / numWallRaycasts;
        for (int i = 0; i < numWallRaycasts; i++)
        {
            float currentAngle = i * angleStep;
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 rayDirection = transform.rotation * rotation * Vector3.forward;
            // Simplified direction calculation:
            // Vector3 rayDirection = Quaternion.Euler(0, currentAngle, 0) * transform.forward;

            //Vector3 rayFrom = transform.position;
            Vector3 rayFrom = transform.position - Vector3.up * 0.5f;
            RaycastHit hit;
            bool wallDetected = false;
            float distanceToHit = raycastDistance; // Default to max distance

            if (Physics.Raycast(rayFrom, rayDirection, out hit, raycastDistance))
            {
                // Consider layers or specific tags if more than just "Wall" exists
                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    wallDetected = true;
                    distanceToHit = hit.distance;
                }
                // Optional: Observe other object types? (e.g., obstacles, cover)
                // sensor.AddObservation(hit.collider.CompareTag("Obstacle")); // Example
            }
            sensor.AddObservation(wallDetected); // Bool observation
            sensor.AddObservation(distanceToHit / raycastDistance); // Normalized distance
        }

        // Bullet tracking
        bulletTracker.ClearTrackedBulletList();
        bulletTracker.DetectBullets();
        // track 3 bullets
        for (int i = 0; i < bulletTrackCount; i++)
        {
            if (i < bulletTracker.trackedBullets.Count)
            {
                Vector3 bulletPosition = bulletTracker.trackedBullets[i].position;
                Vector3 aiPosition = transform.position;
                sensor.AddObservation(bulletPosition - aiPosition);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
            }
        }

        // Target Observations
        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center;
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                            Vector3.Distance(transform.position, target.transform.position) <= visionRange;


        if (playerInFOV)
        {
            Ray ray = new Ray(transform.position, directionToPlayer);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, visionRange))
            {
                if (hit.collider.CompareTag("Player")) 
                {
                    lastKnownPlayerLocation = hit.point;
                    playerVisible = true;
                }
            }
        }
        sensor.AddObservation(playerVisible);

        Vector3 relativeLastKnown = lastKnownPlayerLocation - transform.position;
        // Optionally normalize or clamp magnitude
        sensor.AddObservation(relativeLastKnown.normalized); // Direction only
        sensor.AddObservation(relativeLastKnown.magnitude / visionRange); // Normalized distance (approx)
                                                                          // OR just observe the possibly clamped/normalized relative vector:
                                                                          // sensor.AddObservation(Vector3.ClampMagnitude(relativeLastKnown, visionRange) / visionRange);

        sensor.AddObservation(weapon.ReadyToFire);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        // Rotation
        yawInput = actions.ContinuousActions[2];   // Yaw: Value from -1 to 1
        pitchInput = actions.ContinuousActions[3]; // Pitch: Value from -1 to 1
        
        // Movement
        float moveForwardBackward = actions.ContinuousActions[0]; // Forward/Backward movement
        float moveLeftRight = actions.ContinuousActions[1];     // Strafing movement

        Vector3 moveDirectionForward = transform.forward * moveForwardBackward * moveSpeed;
        Vector3 moveDirectionStrafe = transform.right * moveLeftRight * moveSpeed;

        finalMoveDirection = moveDirectionForward + moveDirectionStrafe;

        Vector3 playerCenter = target.GetComponentInChildren<CapsuleCollider>().bounds.center;
        Vector3 directionToPlayer = (playerCenter - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        bool playerInFOV = angleToPlayer < fovAngle / 2 &&
                           Vector3.Distance(transform.position, target.transform.position) <= visionRange;

        if (playerInFOV)
        {
            AddReward(0.01f);

        } else
        {
        }

        bool fire = actions.DiscreteActions[0] == 1;

        if (fire)
        {
            input.TriggerAction();
        }

    }

    private void FixedUpdate()
    {
        totalTime += Time.fixedDeltaTime;
        if (totalTime > episodeDuration)
        {
            EndEpisode();
        }
        
        rb.AddForce(finalMoveDirection - rb.linearVelocity, ForceMode.VelocityChange);
        
        float yawDegrees = yawInput * turnSpeed * Time.fixedDeltaTime;
        float pitchDegrees = pitchInput * turnSpeed * Time.fixedDeltaTime;

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