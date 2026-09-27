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

    public Transform hunterLeftHand;
    public Transform hunterRightHand;
    private Transform player; // Посилання на гравця
    public float attackCooldown = 4f;
    private float cooldownTimer;
    public bool isPike = false; // ��кщо ворог - щука

    public Animator animator;
    private int dir = 1;

    private bool isAttacking = false;
    private bool isRotating = false;
    public bool isTargeting = false;

    public float bulletSpeed = 5f;
    public float attackDistance = 5f;
    public float leftHandCorrection = -27f;
    public float rightHandCorrection = 20f;
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
                // RotateToTarget();

                yield return null;
            }

            agent.SetDestination(pointB.position);

            while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
            {
                // RotateToTarget();

                yield return null;
            }
        }
    }

    void RotateToTarget()
    {
        Vector3 direction = player.position - hunterLeftHand.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            // Обчислюємо базовий кут нахилу
            float baseAngle = Mathf.Atan2(direction.y, new Vector2(direction.x, direction.z).magnitude) * Mathf.Rad2Deg;

            // Додаємо +20f (або -20f, якщо треба в інший бік) лише для лівої руки
            float leftHandTargetAngle = baseAngle + leftHandCorrection;
            float rightHandTargetAngle = baseAngle + rightHandCorrection;

            // Зберігаємо поточні кути
            Vector3 leftHandAngles = hunterLeftHand.localEulerAngles;
            Vector3 rightHandAngles = hunterRightHand.localEulerAngles;

            // Змінюємо ЛИШЕ ось Z, інші осі (X та Y) залишаємо незмінними
            leftHandAngles.x = Mathf.LerpAngle(leftHandAngles.x, -leftHandTargetAngle, 10f * Time.deltaTime);
            rightHandAngles.z = Mathf.LerpAngle(rightHandAngles.z, rightHandTargetAngle, 10f * Time.deltaTime);
            // leftHandAngles.y = 0f;

            hunterLeftHand.localEulerAngles = leftHandAngles;
            hunterRightHand.localEulerAngles = rightHandAngles;
        }
    }

    void RotateHunterToPlayer()
    {
        Vector3 direction = player.position - hunter.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            hunter.rotation = Quaternion.RotateTowards(
                hunter.rotation,
                targetRotation,
                360f * Time.deltaTime
            );
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
            animator.SetBool("IsHunterWalk", true);
            //animator.SetBool("IsHunterAttack", false);
            return;
        }


        agent.isStopped = true;

        RotateHunterToPlayer();

        if (isTargeting)
        {

            RotateToTarget();
        }

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
            animator.SetBool("IsHunterWalk", false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("mmm Player OUT");
            isAttacking = false;
            // agent.isStopped = false;
            //animator.SetBool("IsHunterAttack", false);
            agent.SetDestination(pointB.position);
        }
    }
    void Attack()
    {
        Debug.Log("mmm ATTACK");
        animator.SetBool("IsHunterWalk", false);
        //animator.SetBool("IsHunterAttack", true);
        FireProjectile();
        //  SoundManager.Instance.PlayOneShot(SoundManager.Instance.shotSound);
        agent.isStopped = true;





        StartCoroutine(ResetAttackAnim());
    }

    public void FireProjectile()
    {
        StartCoroutine(IsTargeting());



    }

    IEnumerator IsTargeting()
    {
        agent.isStopped = true;
        animator.SetBool("IsHunterWalk", false);
        animator.enabled = false;

        isTargeting = true;

        yield return new WaitForSeconds(2f);

        // if (player == null)
        // {
        //     isTargeting = false;
        //     animator.enabled = true;
        //     agent.isStopped = false;
        //     yield break;
        // }

        Vector3 direction = player.position - firePoint.position;
        direction.y = 0f;
        direction.Normalize();

        GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.LookRotation(direction));
        bullet.GetComponent<Rigidbody>().linearVelocity = direction * bulletSpeed;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        animator.enabled = true;
        agent.isStopped = false;

    }

    IEnumerator ResetAttackAnim()
    {
        yield return new WaitForSeconds(0.6f);

        //animator.SetBool("IsHunterAttack", false);
    }
}
