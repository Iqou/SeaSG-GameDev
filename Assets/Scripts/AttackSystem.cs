using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine.InputSystem;
using UnityEngine;

public class AttackSystem : MonoBehaviour
{
    public Transform attackOrigin;
    public float attackRadius = 1f;
    public LayerMask enemyMask;

    public int attackDamage = 25;

    public Animator animator;
    public float attackDuration = 1f;
    public bool isAttacking { get; private set; }


    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                // If an "Attack" state exists on layer 0 force-play it to avoid being interrupted by walk transitions
                if (animator.HasState(0, Animator.StringToHash("Attack")))
                {
                    animator.Play("Attack", 0, 0f);
                }
            }
            
            StartCoroutine(EndAttackAfterDelay());
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyMask);
            foreach (var enemy in enemiesInRange)
            {
                //enemy.GetComponent<HealthManager>().TakeDamage(attackDamage);
                if (enemy.CompareTag("Enemy"))
                {
                    enemy.GetComponent<HealthManager>().TakeDamage(attackDamage);
                }
            }
            //SoundManager.Instance.PlaySound2D("Attack");
        }

    }
    private void OnDrawGizmos()
    {
        if (attackOrigin != null)
        {
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
        }
    }

    private IEnumerator EndAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackDuration);
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
        }
    }
}
