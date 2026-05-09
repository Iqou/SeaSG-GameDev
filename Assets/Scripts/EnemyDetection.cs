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

    // =========================
    // START
    // =========================
    private void Start()
    {
        currentState = EnemyState.Idle;
    }

    // =========================
    // UPDATE
    // =========================
    private void Update()
    {
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
                    jump: false,
                    attack: false
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
                    jump: false,
                    attack: false
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
                    jump: false,
                    attack: true
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
        enemy.position = Vector2.MoveTowards(
            enemy.position,
            player.transform.position,
            moveSpeed * Time.deltaTime
        );

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

            lastAttackTime = Time.time;
        }
    }

    // =========================
    // DETECTION
    // =========================
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            playerDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == player)
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
        bool jump,
        bool attack
    )
    {
        animator.SetBool("idle", idle);
        animator.SetBool("Walk", Walk);
        animator.SetBool("Jump", jump);
        animator.SetBool("attack", attack);
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