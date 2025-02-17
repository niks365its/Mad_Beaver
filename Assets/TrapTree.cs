using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTree : MonoBehaviour
{
    public float damageToPlayer = 30f;
    public float knockbackForce = 8f;
    public Animator animator;
    private bool hasCollided = false;
    private float cooldownTimer = 0f;
    private float cooldownDuration = 1f; // Час між спрацьовуваннями

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasCollided)
        {
            animator.SetBool("IsJump", false);
            HealthBar healthBar = collision.gameObject.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(damageToPlayer);
            }

            hasCollided = true;

            SoundManager.Instance.PlayOneShot(SoundManager.Instance.treesTrapSound);

            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.velocity = Vector2.zero; // Скидаємо поточну швидкість
                playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            }


            cooldownTimer = cooldownDuration; // Запускаємо таймер
        }


    }
    void OnCollisionExit(Collision collision)
    {
        hasCollided = false;
    }
}
