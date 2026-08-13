using System.Collections;
using UnityEngine;

public class HunterControl3 : MonoBehaviour
{
    public enum State
    {
        Chase,
        Attack
    }

    [Header("Movement")]
    public float hunterSpeed = 3.5f;
    private float chaseSpeed;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float bulletSpeed = 5f;
    public float onPlayerDamage = 10f;
    public float maxDist = 10f;

    [Header("Refs")]
    public Transform firePoint;
    public Transform detectionPoint;
    public GameObject projectile;
    public Animator animator;
    public GameObject crocodileBody;
    public Control3 playerController;

    private Transform player;
    private GameObject playerBody;
    private State state;
    private float cooldownTimer;

    void Start()
    {
        playerBody = GameObject.FindGameObjectWithTag("Player");

        if (playerBody != null)
        {
            player = playerBody.transform.Find("TailPoint");
        }

        state = State.Chase;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Chase:
                Chase();
                break;

            case State.Attack:
                AttackState();
                break;
        }
    }

    #region STATES

    void Chase()
    {
        if (player == null || playerController == null)
            return;

        float dist = Vector2.Distance(
            detectionPoint.position,
            player.position);

        float playerSpeed = playerController.currentSpeed;

        // Якщо гравець у радіусі атаки — зупиняємо переслідування
        if (dist <= attackRange)
        {
            if (playerSpeed < hunterSpeed)
            {
                chaseSpeed = playerSpeed;
            }
            else
            {
                chaseSpeed = hunterSpeed;
            }

            Debug.Log(
                $"xxxChase Speed: {chaseSpeed:F2}" +
                $"Player Speed: {playerSpeed:F2}" +
                $"Distance: {dist:F2}" +
                $"Attack Range: {attackRange:F2}"
            );

            state = State.Attack;
            return;
        }

        // Визначаємо швидкість переслідування
        if (dist >= maxDist)
        {
            if (playerSpeed < hunterSpeed)
            {
                chaseSpeed = hunterSpeed;
            }
            else
            {
                chaseSpeed = playerSpeed;
            }
        }
        else
        {
            chaseSpeed = hunterSpeed;
        }

        MoveTo(player.position, chaseSpeed);
        Debug.Log("xxxDistance: " + dist);
    }

    void AttackState()
    {
        if (player == null)
            return;

        float dist = Vector2.Distance(
            detectionPoint.position,
            player.position);

        FacePlayer();

        if (dist > attackRange)
        {
            state = State.Chase;
            return;
        }

        if (cooldownTimer <= 0f)
        {
            DoAttack();
            cooldownTimer = attackCooldown;
        }
    }

    #endregion

    #region ACTIONS

    void MoveTo(Vector3 target, float speed)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        FaceDirection(target);
    }

    void FaceDirection(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        if (Mathf.Abs(dir.x) < 0.01f)
            return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
        transform.localScale = scale;
    }

    void FacePlayer()
    {
        if (player == null)
            return;

        FaceDirection(player.position);
    }

    void DoAttack()
    {
        animator.SetBool("IsHunterAttack", true);

        crocodileBody.GetComponent<SpriteRenderer>().color =
            new Color(1f, 1f, 1f, 0f);

        StartCoroutine(ResetAnim());

        HealthBar enemyHealth = playerBody.GetComponent<HealthBar>();

        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(onPlayerDamage);
        }
    }

    IEnumerator ResetAnim()
    {
        yield return new WaitForSeconds(0.5f);

        animator.SetBool("IsHunterAttack", false);

        crocodileBody.GetComponent<SpriteRenderer>().color =
            new Color(1f, 1f, 1f, 1f);
    }

    #endregion

    #region TRIGGERS

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerBody = other.gameObject;
            player = other.transform.Find("TailPoint");
        }

        if (other.CompareTag("Obstacle"))
        {
            crocodileBody.GetComponent<SpriteRenderer>().color =
                new Color(1f, 1f, 1f, 0.5f);
        }

        Debug.Log("wwwENTER: " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            crocodileBody.GetComponent<SpriteRenderer>().color =
                new Color(1f, 1f, 1f, 1f);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                detectionPoint.position,
                attackRange);

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(
                detectionPoint.position,
                0.1f);
        }

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(
                firePoint.position,
                0.1f);
        }
    }
}