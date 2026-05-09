using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    // =========================
    // ENEMY STATES
    // =========================
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    // =========================
    // REFERENCES
    // =========================
    [Header("References")]
    public GameObject player;
    public Transform enemy;
    public Animator animator;

    // =========================
    // MOVEMENT SETTINGS
    // =========================
    [Header("Movement")]
    public float moveSpeed = 3f;

    // =========================
    // ATTACK SETTINGS
    // =========================
    [Header("Attack")]
    public float attackRange = 1.5f;
    public int damageAmount = 10;
    public float attackCooldown = 1f;

    // =========================
    // PRIVATE VARIABLES
    // =========================
    private EnemyState currentState;

    private bool playerDetected = false;
    private float lastAttackTime;
    private Rigidbody2D enemyRb;
    private EnemyState previousState;

    // =========================
    // START
    // =========================
    private void Start()
    {
        currentState = EnemyState.Idle;
        previousState = currentState;
        if (enemy != null)
        {
            enemyRb = enemy.GetComponent<Rigidbody2D>();
        }

        if (player == null)
        {
            Debug.LogWarning("EnemyDetection: 'player' reference is not set in the Inspector.");
        }

        if (enemy == null)
        {
            Debug.LogWarning("EnemyDetection: 'enemy' reference is not set in the Inspector.");
        }

        if (animator == null)
        {
            Debug.LogWarning("EnemyDetection: 'animator' reference is not set in the Inspector.");
        }

        // state entry handling
        if (currentState != previousState)
        {
            Debug.Log($"Enemy state: {previousState} -> {currentState}");
            if (currentState == EnemyState.Attack && animator != null)
            {
                Debug.Log($"EnemyDetection: entering Attack state - setting animator trigger. HasState(Attack)={animator.HasState(0, Animator.StringToHash("Attack"))}");
                animator.SetTrigger("Attack");
                var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                Debug.Log($"Animator current state shortNameHash={stateInfo.shortNameHash}, normalizedTime={stateInfo.normalizedTime}, isInTransition={animator.IsInTransition(0)}");
            }
            previousState = currentState;
        }
    }

    // =========================
    // UPDATE
    // =========================
    private void Update()
    {
        if (player == null || enemy == null || animator == null)
            return;

        float distanceToPlayer = Vector2.Distance(
            enemy.position,
            player.transform.position
        );

        switch (currentState)
        {
            // =========================
            // IDLE STATE
            // =========================
            case EnemyState.Idle:

                SetAnimation(
                    idle: true,
                    Walk: false,
                    jump: false
                );

                // Detect player
                if (playerDetected)
                {
                    currentState = EnemyState.Chase;
                }

                break;

            // =========================
            // CHASE STATE
            // =========================
            case EnemyState.Chase:

                SetAnimation(
                    idle: false,
                    Walk: true,
                    jump: false
                );

                // Player escaped
                if (!playerDetected)
                {
                    currentState = EnemyState.Idle;
                    break;
                }

                MoveToPlayer();

                // Player close enough to attack
                if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }

                break;

            // =========================
            // ATTACK STATE
            // =========================
            case EnemyState.Attack:

                SetAnimation(
                    idle: false,
                    Walk: false,
                    jump: false
                );

                // Player too far
                if (distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chase;
                    break;
                }

                AttackPlayer();
           

                break;
        }
    }

    // =========================
    // MOVE TO PLAYER
    // =========================
    private void MoveToPlayer()
    {
        Vector2 direction =
            player.transform.position - enemy.position;

        // -------------------------
        // MOVEMENT ANIMATION
        // -------------------------
        bool movingX =
            Mathf.Abs(direction.x) > 0.1f;

        bool movingY =
            Mathf.Abs(direction.y) > 0.5f;

        animator.SetBool("Walk", movingX);
        animator.SetBool("Jump", movingY);

        // -------------------------
        // MOVE ENEMY
        // -------------------------
        Vector2 targetPos = player.transform.position;
        if (enemyRb != null)
        {
            Vector2 next = Vector2.MoveTowards(enemyRb.position, targetPos, moveSpeed * Time.deltaTime);
            enemyRb.MovePosition(next);
        }
        else
        {
            enemy.position = Vector2.MoveTowards(
                enemy.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
        }

        // -------------------------
        // FLIP SPRITE
        // -------------------------
        if (direction.x > 0)
        {
            enemy.localScale = new Vector3(
                Mathf.Abs(enemy.localScale.x),
                enemy.localScale.y,
                enemy.localScale.z
            );
        }
        else if (direction.x < 0)
        {
            enemy.localScale = new Vector3(
                -Mathf.Abs(enemy.localScale.x),
                enemy.localScale.y,
                enemy.localScale.z
            );
        }
    }

    // =========================
    // ATTACK PLAYER
    // =========================
    private void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            HealthManager health =
                player.GetComponent<HealthManager>();

            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
        }
    }

    // =========================
    // DETECTION
    // =========================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = true;
            if (player == null)
                player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerDetected = false;
        }
    }

    // =========================
    // ANIMATION HELPER
    // =========================
    private void SetAnimation(
        bool idle,
        bool Walk,
        bool jump
    )
    {
        // Use consistent parameter names in the Animator: Idle, Walk, Jump
        animator.SetBool("Idle", idle);
        animator.SetBool("Walk", Walk);
        animator.SetBool("Jump", jump);
    }

    // =========================
    // GIZMOS
    // =========================
    private void OnDrawGizmos()
    {
        if (enemy == null)
            return;

        // -------------------------
        // ATTACK RANGE
        // -------------------------
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            enemy.position,
            attackRange
        );

        // -------------------------
        // DETECTION RANGE
        // -------------------------
        CircleCollider2D detection =
            GetComponent<CircleCollider2D>();

        if (detection != null)
        {
            Gizmos.color = Color.yellow;

            float radius =
                detection.radius *
                detection.transform.lossyScale.x;

            Gizmos.DrawWireSphere(
                detection.transform.position,
                radius
            );
        }
    }
}