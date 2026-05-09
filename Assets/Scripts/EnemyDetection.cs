using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    [Header("References")]
    public GameObject player;
    public Transform enemy;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Attack")]
    public float attackRange = 1.5f;
    public int damageAmount = 10;
    public float attackCooldown = 1f;

    private EnemyState currentState;

    private bool playerDetected = false;
    private float lastAttackTime;

    private void Start()
    {
        currentState = EnemyState.Idle;
    }

    private void Update()
    {
        float distanceToPlayer = Vector2.Distance(
            enemy.position,
            player.transform.position
        );

        switch (currentState)
        {
            case EnemyState.Idle:

                animator.SetBool("idle", true);
                animator.SetBool("Movement", false);
                animator.SetBool("Jump", false);
                animator.SetBool("attack", false);

                if (playerDetected)
                {
                    currentState = EnemyState.Chase;
                }

                break;

            case EnemyState.Chase:

                animator.SetBool("attack", false);
                animator.SetBool("Movement", true);

                if (!playerDetected)
                {
                    currentState = EnemyState.Idle;
                    break;
                }

                MoveToPlayer();

                if (distanceToPlayer <= attackRange)
                {
                    currentState = EnemyState.Attack;
                }

                break;

            case EnemyState.Attack:

                animator.SetBool("attack", true);
                animator.SetBool("Movement", false);
                animator.SetBool("Jump", false);
                animator.SetBool("idle", false);

                if (distanceToPlayer > attackRange)
                {
                    animator.SetBool("attack", false);

                    currentState = EnemyState.Chase;
                    break;
                }

                AttackPlayer();

                break;
        }
    }

    private void MoveToPlayer()
    {
        Vector2 direction =
            player.transform.position - enemy.position;

        // Movement checks
        bool movingX = Mathf.Abs(direction.x) > 0.1f;
        bool movingY = Mathf.Abs(direction.y) > 0.5f;

        // Animator parameters
        animator.SetBool("Movement", movingX);
        animator.SetBool("Jump", movingY);
        animator.SetBool("idle", !movingX && !movingY);

        // Move enemy
        enemy.position = Vector2.MoveTowards(
            enemy.position,
            player.transform.position,
            moveSpeed * Time.deltaTime
        );

        // Flip sprite
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

    private void OnDrawGizmos()
    {
        if (enemy == null)
            return;

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            enemy.position,
            attackRange
        );

        // Detection range
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