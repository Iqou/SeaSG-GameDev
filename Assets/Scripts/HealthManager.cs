using UnityEngine;
using UnityEngine.SceneManagement;
public class HealthManager : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        
         if (healthBar != null)
         {
             healthBar.SetMaxHealth(maxHealth);
         }
        
    }

    void Update()
    {
        // Fitur tes damage untuk diri sendiri (bisa dihapus nanti kalau sudah tidak dipakai)
        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    TakeDamage(20);
        //}
    }

    // Ubah menjadi PUBLIC agar bisa dipanggil oleh script AttackSystem milik Anda
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Terkena damage! HP sekarang: " + currentHealth); // Bantuan visual di Console

        
        if (healthBar != null)
        {
            healthBar.SetCurrentHealth(currentHealth);
        }
        
        
        if (currentHealth <= 0)
        {
            Debug.Log("Karakter Mati!");
            //Destroy(gameObject); // Hapus objek karakter dari scene
            Destroy(gameObject);
        }
    }
}