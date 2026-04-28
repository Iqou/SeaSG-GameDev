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


    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(attackOrigin.position, attackRadius, enemyMask);
            foreach (var enemy in enemiesInRange)
            {
                enemy.GetComponent<HealthManager>().TakeDamage(attackDamage);
            }
        }

    }
    private void OnDrawGizmos()
    {
        if (attackOrigin != null)
        {
            Gizmos.DrawWireSphere(attackOrigin.position, attackRadius);
        }
    }
}
