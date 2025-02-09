using UnityEngine;

public class TreesTrap : MonoBehaviour
{
    public float knockbackForce = 5f; // Сила відкидання
    public float onPlayerDamage = 10f; // Кількість пошкоджень
    public float onEnemyDamage = 10f; // Кількість пошкоджень ворога

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.velocity = Vector2.zero; // Скидаємо поточну швидкість
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }

            HealthBar healthBar = collision.gameObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(onPlayerDamage); // Застосувати пошкодження
            }
        }


    }
}
