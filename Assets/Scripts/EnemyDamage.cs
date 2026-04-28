using UnityEngine;


public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 15;


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {

            HealthManager playerHealth = collision.gameObject.GetComponent<HealthManager>();


            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}