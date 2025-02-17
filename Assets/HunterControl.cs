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

    public Animator animator;

    private bool isAttacking = false;
    private bool readyToShoot = false; // Чекає на завершення зміни анімації

    // public AudioSource audioSource;
    // public AudioClip shotSound;

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

        else if (player != null)
        {
            Vector2 hunterPosition = (Vector2)hunter.transform.position;
            // Динамічне оновлення анімації в залежності від кута
            UpdateAnimation(hunterPosition, player.position);

            // Якщо анімація оновлена, виконуємо постріл
            if (readyToShoot && cooldownTimer <= 0f)
            {
                Attack(hunterPosition, player.position);
                cooldownTimer = attackCooldown;
                readyToShoot = false; // Чекаємо наступного оновлення анімації
            }
        }

        cooldownTimer -= Time.deltaTime;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isAttacking = true;

            // Повернення в напрямку гравця
            Vector3 playerDirection = player.position - transform.position;
            if (playerDirection.x != 0)
            {
                transform.localScale = new Vector3(Mathf.Sign(playerDirection.x), 1, 1);
            }

            // Спочатку оновлюємо анімацію, а потім чекаємо, коли можна буде стріляти
            Vector2 hunterPosition = (Vector2)hunter.transform.position;
            UpdateAnimation(hunterPosition, player.position);
            readyToShoot = true;
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            readyToShoot = false;
            animator.SetBool("IsHunterAttack", false);
            animator.SetBool("IsHunterAttackDown", false);
        }
    }


    void Attack(Vector2 hunterPosition, Vector2 targetPosition)
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.shotSound);
        // animator.SetBool("IsHunterAttack", true);
        GameObject bullet = Instantiate(projectile, firePoint.position, projectile.transform.rotation);
        // Vector2 direction = (targetPosition - (Vector2)firePoint.position).normalized;
        Vector2 direction = (targetPosition - hunterPosition).normalized;

        bullet.GetComponent<Rigidbody2D>().velocity = direction * 5f; // Швидкість кулі

    }

    void UpdateAnimation(Vector2 hunterPosition, Vector2 targetPosition)
    {
        Vector2 directionToPlayer = (targetPosition - hunterPosition).normalized;
        float angle = Vector2.Angle(Vector2.right, directionToPlayer); // Кут між напрямком атаки і горизонталлю

        if (angle <= 25f)
        {
            animator.SetBool("IsHunterAttack", true);
            animator.SetBool("IsHunterAttackDown", false);
        }
        else
        {
            animator.SetBool("IsHunterAttack", false);
            animator.SetBool("IsHunterAttackDown", true);
        }
    }
}
