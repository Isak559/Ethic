
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;

    [Header("Detection")]
    public float detectionRange = 15f;
    public float stopChasingRange = 20f;
    public float combatDistance = 7f;

    [Header("Patrolling")]
    public float patrolSpeed = 2f;
    public float patrolWaitTime = 2f;

    private int patrolIndex = 0;
    private float waitTimer;
    private bool waiting;

    [Header("Combat Movement")]
    public float chaseSpeed = 4f;
    public float strafeSpeed = 2.5f;
    public float strafeDistance = 3f;
    public float strafeInterval = 2f;
    public float rotationSpeed = 8f;

    private float strafeTimer;
    private int strafeDirection = 1;
    private bool detectedPlayer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = patrolSpeed;
        agent.updateRotation = false;

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
                player = obj.transform;
        }

        strafeTimer = strafeInterval;
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (!detectedPlayer && distance <= detectionRange)
            detectedPlayer = true;

        if (detectedPlayer && distance > stopChasingRange)
            detectedPlayer = false;

        if (detectedPlayer)
        {
            CombatMovement(distance);
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0f;

        if (patrolPoints.Length == 0)
        {
            agent.ResetPath();
            return;
        }

        if (waiting)
        {
            agent.ResetPath();

            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                waiting = false;
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            }

            return;
        }

        agent.SetDestination(patrolPoints[patrolIndex].position);

        if (!agent.pathPending &&
            agent.remainingDistance <= 0.5f)
        {
            waiting = true;
            waitTimer = patrolWaitTime;
        }

        FaceMovementDirection();
    }

    void CombatMovement(float distance)
    {
        FacePlayer();

        if (distance > combatDistance)
        {
            agent.speed = chaseSpeed;
            agent.stoppingDistance = combatDistance * 0.8f;
            agent.SetDestination(player.position);
        }
        else
        {
            Strafe();
        }
    }

    void Strafe()
    {
        agent.speed = strafeSpeed;
        agent.stoppingDistance = 0f;

        strafeTimer -= Time.deltaTime;

        if (strafeTimer <= 0)
        {
            strafeDirection = Random.value > 0.5f ? 1 : -1;
            strafeTimer = strafeInterval;
        }

        Vector3 directionToPlayer =
            (player.position - transform.position).normalized;

        Vector3 right = Vector3.Cross(
            Vector3.up,
            directionToPlayer
        ).normalized;

        Vector3 strafeTarget =
            transform.position +
            right * strafeDirection * strafeDistance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            strafeTarget,
            out hit,
            2f,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    void FaceMovementDirection()
    {
        Vector3 direction = agent.velocity;
        direction.y = 0;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
