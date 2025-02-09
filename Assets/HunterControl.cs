using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterControl : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Transform target;

    public GameObject projectile;
    public Transform firePoint;
    public float attackCooldown = 2f;
    private float cooldownTimer;

    public Animator animator;

    private bool isAttacking = false;

    void Start()
    {
        target = pointA;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hunter"), LayerMask.NameToLayer("Player"), true);
    }

    void Update()
    {
        if (!isAttacking)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            // Повернення в напрямку руху
            Vector3 direction = target.position - transform.position;
            if (direction.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(direction.x), 1, 1);
            }

            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                if (target == pointA)
                {
                    target = pointB;
                }
                else
                {
                    target = pointA;
                }
            }
        }

        cooldownTimer -= Time.deltaTime;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && cooldownTimer <= 0f)
        {
            isAttacking = true;

            // Повернення в напрямку плеєра
            Vector3 playerDirection = other.transform.position - transform.position;
            if (playerDirection.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(playerDirection.x), 1, 1);
            }

            animator.SetBool("IsHunterAttack", true);
            Attack(other.transform.position);
            cooldownTimer = attackCooldown;
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            animator.SetBool("IsHunterAttack", false);
        }
    }

    /* public void StopAttack()
     {
         animator.SetBool("IsHunterAttack", false);
     } */

    void Attack(Vector2 targetPosition)
    {
        animator.SetBool("IsHunterAttack", true);
        GameObject bullet = Instantiate(projectile, firePoint.position, projectile.transform.rotation);
        Vector2 direction = (targetPosition - (Vector2)firePoint.position).normalized;
        bullet.GetComponent<Rigidbody2D>().velocity = direction * 5f; // Швидкість каменя

    }
}
