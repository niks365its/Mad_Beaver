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
    public Transform hunter;
    private Transform player; // Посилання на гравця
    public float attackCooldown = 2f;
    private float cooldownTimer;
    public bool isPike = false; // ��кщо ворог - щука

    public Animator animator;
    private int dir = 1;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    public float bulletSpeed = 5f;

    void Start()
    {
        if (isPike)
        {
            dir = -1;
        }

        target = pointA;
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Hunter"), LayerMask.NameToLayer("Player"), true);
    }

    void Update()
    {


        if (!isAttacking)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            Vector3 direction = target.position - transform.position;


            if (direction.x != 0)
            {

                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(dir * direction.x);
                transform.localScale = currentScale;
            }

            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                target = target == pointA ? pointB : pointA;
            }
        }
        else if (player != null && cooldownTimer <= 0f)
        {
            Attack();
            cooldownTimer = attackCooldown;
        }

        cooldownTimer -= Time.deltaTime;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isAttacking = true;

            Vector3 playerDirection = player.position - transform.position;
            if (playerDirection.x != 0)
            {
                // transform.localScale = new Vector3(Mathf.Sign(playerDirection.x), 1, 1);
                Vector3 currentScale = transform.localScale;
                currentScale.x = Mathf.Abs(currentScale.x) * Mathf.Sign(dir * playerDirection.x);
                transform.localScale = currentScale;
            }


        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            // animator.SetBool("IsHunterAttack", false);
        }
    }
    void Attack()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.shotSound);

        Vector2 direction = ((Vector2)player.position - (Vector2)firePoint.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Обчислення кута

        GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.Euler(0, 0, angle + 90));
        bullet.GetComponent<Rigidbody2D>().velocity = direction * bulletSpeed;
        animator.SetBool("IsHunterAttack", true);
        StartCoroutine(ResetAttackAnim());
    }

    IEnumerator ResetAttackAnim()
    {
        yield return new WaitForSeconds(0.6f); // час залежить від вашої анімації
        animator.SetBool("IsHunterAttack", false);
    }
}
