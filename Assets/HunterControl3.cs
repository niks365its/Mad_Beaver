using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterControl3 : MonoBehaviour
{
    // Перелік станів AI мисливця (машина станів)
    public enum State
    {
        Patrol,   // Патрулювання між точками
        Detect,   // Перевірка чи гравець у зоні
        Chase,    // Переслідування
        Attack    // Атака
    }

    [Header("Movement")]
    public float patrolSpeed = 2f;   // Швидкість руху під час патруля
    public float chaseSpeed = 3.5f;  // Швидкість переслідування
    public Transform[] patrolPoints; // Масив точок маршруту

    [Header("Combat")]
    public float attackRange = 1.5f;  // Дистанція атаки
    public float detectRange = 5f;    // Дистанція виявлення гравця
    public float attackCooldown = 2f; // Затримка між атаками
    public float bulletSpeed = 5f;    // Швидкість снаряда

    [Header("Refs")]
    public Transform firePoint;       // Точка, з якої вилітає снаряд
    public Transform detectionPoint;  // Точка, від якої рахується атака/дистанція
    public GameObject projectile;     // Префаб снаряда
    public Animator animator;         // Аніматор персонажа
    public GameObject crocodileBody;  // Об’єкт тіла (для зміни кольору/прозорості)

    private Transform player;         // Поточна ціль (гравець)
    private State state;              // Поточний стан AI
    private int patrolIndex;          // Індекс поточної точки патруля
    private float cooldownTimer;      // Таймер кулдауну атаки

    void Update()
    {
        // Лог для дебагу: показує стан і чи є гравець
        Debug.Log($"wwwState: {state}, Player: {(player ? player.name : "NULL")}");

        // Відлік кулдауну атаки
        cooldownTimer -= Time.deltaTime;

        // Основна машина станів (FSM)
        switch (state)
        {
            case State.Patrol:
                Patrol();        // Патрулювання
                break;

            case State.Detect:
                Detect();        // Перевірка видимості гравця
                break;

            case State.Chase:
                Chase();         // Переслідування
                break;

            case State.Attack:
                AttackState();   // Атака
                break;
        }
    }

    #region STATES

    void Patrol()
    {
        // Поточна ціль патрулювання
        Transform target = patrolPoints[patrolIndex];

        // Рух до точки патруля
        MoveTo(target.position, patrolSpeed);

        // Якщо дійшли до точки — переходимо до наступної
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        // Якщо гравець існує і він у зоні виявлення — переходимо в Detect
        if (player != null &&
            Vector2.Distance(detectionPoint.position, player.position) <= detectRange)
        {
            state = State.Detect;
        }
    }

    void Detect()
    {
        // Якщо гравець зник — повертаємось до патруля
        if (player == null)
        {
            state = State.Patrol;
            return;
        }

        // Якщо гравець у зоні — починаємо переслідування
        if (Vector2.Distance(detectionPoint.position, player.position) <= detectRange)
        {
            state = State.Chase;
        }
        else
        {
            // Якщо вийшов із зони — назад у патруль
            state = State.Patrol;
        }
    }

    void Chase()
    {
        // Перевірка: чи існує гравець взагалі в сцені / в зоні видимості
        if (player == null)
        {
            // Якщо гравця немає — AI не має цілі, тому повертаємось у патруль
            state = State.Patrol;

            // Вихід з функції, щоб не виконувати подальший код (щоб не було null reference)
            return;
        }

        // Обчислення дистанції між мисливцем і гравцем
        // Vector2.Distance = формула: sqrt((x2-x1)^2 + (y2-y1)^2)
        float dist = Vector2.Distance(detectionPoint.position, player.position);

        // Рух до гравця
        MoveTo(player.position, chaseSpeed);

        // Якщо достатньо близько — атака
        if (dist <= attackRange)
        {
            state = State.Attack;
        }

        // Якщо гравець далеко — втрата цілі
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

        // Відстань рахується від detectionPoint (важливо для точності атаки)
        float dist = Vector2.Distance(detectionPoint.position, player.position);

        // Повертаємося обличчям до гравця
        FacePlayer();

        // Якщо гравець вийшов з радіусу атаки — переслідування
        if (dist > attackRange)
        {
            state = State.Chase;
            return;
        }

        // Якщо кулдаун завершився — атакуємо
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
        // Плавний рух до цілі
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        // Повертаємо персонажа в сторону руху
        FaceDirection(target);
    }

    void FaceDirection(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        // Якщо немає зміщення по X — не перевертати
        if (dir.x == 0) return;

        Vector3 scale = transform.localScale;

        // Переворот спрайта вліво/вправо
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dir.x);

        transform.localScale = scale;
    }

    void FacePlayer()
    {
        if (player == null) return;

        // Повертаємося до гравця
        FaceDirection(player.position);
    }

    void DoAttack()
    {
        // Увімкнути анімацію атаки
        animator.SetBool("IsHunterAttack", true);

        // Робимо тіло прозорим (ефект атаки/зникнення)
        crocodileBody.GetComponent<SpriteRenderer>().color =
            new Color(1f, 1f, 1f, 0f);

        // Через короткий час скинути анімацію
        StartCoroutine(ResetAnim());

        // Розрахунок напрямку пострілу
        Vector2 dir = ((Vector2)player.position - (Vector2)firePoint.position).normalized;

        // Створення снаряда
        GameObject bullet = Instantiate(projectile, firePoint.position, Quaternion.identity);

        // Задаємо швидкість руху снаряда
        bullet.GetComponent<Rigidbody2D>().linearVelocity = dir * bulletSpeed;
    }

    IEnumerator ResetAnim()
    {
        // Чекаємо завершення анімації
        yield return new WaitForSeconds(0.5f);

        // Вимикаємо стан атаки
        animator.SetBool("IsHunterAttack", false);

        // Повертаємо видимість тіла
        crocodileBody.GetComponent<SpriteRenderer>().color =
            new Color(1f, 1f, 1f, 1f);
    }

    #endregion

    #region TRIGGERS

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Якщо гравець зайшов у тригер — запам’ятати його як ціль
        if (other.CompareTag("Player"))
        {
            //player = other.transform;
            player = other.transform.Find("TailPoint");
        }

        // Якщо перешкода — робимо напівпрозорим
        if (other.CompareTag("Obstacle"))
        {
            crocodileBody.GetComponent<SpriteRenderer>().color =
                new Color(1f, 1f, 1f, 0.5f);
        }

        Debug.Log("wwwENTER: " + other.name);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Якщо гравець вийшов — забуваємо його і повертаємось до патруля
        if (other.CompareTag("Player"))
        {
            player = null;
            state = State.Patrol;
        }

        // Вихід з перешкоди — повертаємо нормальний колір
        if (other.CompareTag("Obstacle"))
        {
            crocodileBody.GetComponent<SpriteRenderer>().color =
                new Color(1f, 1f, 1f, 1f);
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        // колір для detect range (зелений)
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // колір для attack range (червоний)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionPoint.position, attackRange);

        // точка пострілу (жовта)
        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
        }

        // точка detectionPoint (синя)
        if (detectionPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(detectionPoint.position, 0.1f);
        }
    }
}