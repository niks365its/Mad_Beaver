using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyStickControl : MonoBehaviour

{
    public float onPlayerDamage = 10f; // Кількість пошкоджень
    public float onEnemyDamage = 10f; // Кількість пошкоджень

    void Start()
    {
        Destroy(gameObject, 4f); // Знищення через 5 секунд
    }

    void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Player")) // Перевірити, чи це персонаж
        {
            HealthBar healthBar = collision.gameObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(onPlayerDamage); // Застосувати пошкодження
            }
        }



        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("TreesTrap")) // Якщо це ворог
        {
            EnemyHealthBar enemyHealth = collision.gameObject.GetComponent<EnemyHealthBar>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(onEnemyDamage); // Застосувати пошкодження
            }
        }



        Destroy(gameObject); // Знищення після зіткнення

    }

}