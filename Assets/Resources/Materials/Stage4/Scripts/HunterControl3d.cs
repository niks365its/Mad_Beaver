using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HunterControl3d : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    private Transform target;

    public GameObject projectile;
    public Transform firePoint;
    public Transform hunter;
    private Transform player; // Посилання на гравця
    public float attackCooldown = 4f;
    private float cooldownTimer;
    public bool isPike = false; // ��кщо ворог - щука

    public Animator animator;
    private int dir = 1;

    private bool isAttacking = false;
    public bool IsAttacking => isAttacking;

    public float bulletSpeed = 5f;
    public float attackDistance = 5f;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = hunter.GetComponent<NavMeshAgent>();

    }

    void Start()
    {
        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            agent.SetDestination(pointA.position);

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                RotateToTarget();
                yield return null;
            }

            agent.SetDestination(pointB.position);

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                RotateToTarget();
                yield return null;
            }
        }
    }

    void RotateToTarget()
    {
        Vector3 direction = agent.steeringTarget - hunter.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up) * Quaternion.Euler(0f, 0f, 0f);
            hunter.rotation = Quaternion.Slerp(hunter.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    void Update()
    {
        if (player == null || !isAttacking)
        {
            return;
        }

        float distance = Vector3.Distance(hunter.position, player.position);

        if (distance > attackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("IsHunterAttack", false);
            return;
        }

        agent.isStopped = true;


        if (cooldownTimer <= 0f)
        {
            Debug.Log("mmm ATTACK CALL");
            Attack();
            cooldownTimer = attackCooldown;
        }

        cooldownTimer -= Time.deltaTime;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isAttacking = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("mmm Player OUT");
            isAttacking = false;
            // agent.isStopped = false;
            animator.SetBool("IsHunterAttack", false);
        }
    }
    void Attack()
    {
        Debug.Log("mmm ATTACK");
        animator.SetBool("IsHunterAttack", true);
        //  SoundManager.Instance.PlayOneShot(SoundManager.Instance.shotSound);
        agent.isStopped = true;





        StartCoroutine(ResetAttackAnim());
    }

    public void FireProjectile()
    {
        if (player == null)
        {
            return;
        }

        Vector3 direction = player.position - firePoint.position;
        direction.y = 0f;
        direction.Normalize();

        GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
    }

    IEnumerator ResetAttackAnim()
    {
        yield return new WaitForSeconds(0.6f);

        animator.SetBool("IsHunterAttack", false);
    }
}
