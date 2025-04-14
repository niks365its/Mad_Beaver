using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyStickControl : MonoBehaviour

{
    public float onPlayerDamage = 10f; // Кількість пошкоджень
    public float onEnemyDamage = 10f; // Кількість пошкоджень
    public bool isPlayer = true;
    private float cooldownTimer = 0f;
    private float cooldownDuration = 0.3f; // Час між спрацьовуваннями

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void Start()
    {
        Destroy(gameObject, 4f); // Знищення через 5 секунд
    }

    void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Player") && !isPlayer) // Перевірити, чи це персонаж
        {
            HealthBar healthBar = collision.gameObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(onPlayerDamage); // Застосувати пошкодження

                // audioSource.PlayOneShot(impactSound);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hitSound);
                GetComponent<SpriteRenderer>().enabled = false;
            }
        }



        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("TreesTrap")) && isPlayer && cooldownTimer <= 0f) // Якщо це ворог
        {
            EnemyHealthBar enemyHealth = collision.gameObject.GetComponent<EnemyHealthBar>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(onEnemyDamage); // Застосувати пошкодження
                                                       // audioSource.PlayOneShot(impactSound);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.hitSound);
                GetComponent<SpriteRenderer>().enabled = false;
                 cooldownTimer = cooldownDuration; // Запускаємо таймер перед наступним можливим спрацьовуванням
            }
        }



        Destroy(gameObject, SoundManager.Instance.hitSound.length); // Знищення після зіткнення

    }

}