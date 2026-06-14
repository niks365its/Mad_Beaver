using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterControl3 : MonoBehaviour


{
    public enum State
    {
        Patrol,
        Detect,
        Chase,
        Attack
    }

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public Transform[] patrolPoints;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float detectRange = 5f;
    public float attackCooldown = 2f;
    public float bulletSpeed = 5f;

    [Header("Refs")]
    public Transform firePoint;
    public Transform detectionPoint;
    public GameObject projectile;
    public Animator animator;

    private Transform player;
    private State state;
    private int patrolIndex;
    private float cooldownTimer;

    void Update()
    {
        Debug.Log($"wwwState: {state}, Player: {(player ? player.name : "NULL")}");

        cooldownTimer -= Time.deltaTime;

        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Detect:
                Detect();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                AttackState();
                break;
        }
    }

    #region STATES

    void Patrol()
    {
        Transform target = patrolPoints[patrolIndex];

        MoveTo(target.position, patrolSpeed);

        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        if (player != null &&
            Vector2.Distance(transform.position, player.position) <= detectRange)
        {
            state = State.Detect;
        }
    }

    void Detect()
    {
        if (player == null)
        {
            state = State.Patrol;
            return;
        }

        if (Vector2.Distance(transform.position, player.position) <= detectRange)
        {
            state = State.Chase;
        }
        else
        {
            state = State.Patrol;
        }
    }

    void Chase()
    {
        if (player == null)
        {
            state = State.Patrol;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        MoveTo(player.position, chaseSpeed);

        if (dist <= attackRange)
        {
            state = State.Attack;
        }

        if (dist > detectRange)
        {
            state = State.Patrol;
        }
    }

    void AttackState()
    {
        if (player == null)
        {
            state = State.Patrol;
            return;
        }

        float dist = Vector2.Distance(detectionPoint.position, player.position);

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

        if (dir.x == 0) return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);
        transform.localScale = scale;
    }

    void FacePlayer()
    {
        if (player == null) return;
        FaceDirection(player.position);
    }

    void DoAttack()
    {
        animator.SetBool("IsHunterAttack", true);
        StartCoroutine(ResetAnim());

        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }

    IEnumerator ResetAnim()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsHunterAttack", false);
    }

    #endregion

    #region TRIGGERS

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }

        Debug.Log("wwwENTER: " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            state = State.Patrol;
        }
    }

    #endregion
}