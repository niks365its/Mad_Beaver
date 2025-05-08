using UnityEngine;

public class TurretFollow : MonoBehaviour
{
    public Transform target;
    public Transform hunter;
    public float speed = 2f;
    public bool isPike = false;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        HunterControl hunterScript = hunter.GetComponent<HunterControl>();
        if (target != null && hunter != null && hunterScript.IsAttacking)
        {
            Vector3 direction = target.position - transform.position;

            if (target.position.x > hunter.position.x)
            {
                direction.y = -direction.y;
                direction.x = -direction.x;
            }

            if ((hunter.localScale.x == 1 && target.position.x < hunter.position.x) ||
                (hunter.localScale.x == -1 && target.position.x > hunter.position.x))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                if (isPike)
                {
                    rb.velocity = Vector2.zero; // Зупинити рух
                }
                return;
            }

            if (!isPike)
            {
                float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else
            {
                float targetY = target.position.y + 1f;
                float deltaY = targetY - rb.position.y;

                float velocityY = Mathf.Clamp(deltaY * 5f, -speed, speed); // Можеш підлаштувати множник (5f)

                rb.velocity = new Vector2(0, velocityY);
            }
        }
        else
        {
            if (isPike)
                rb.velocity = Vector2.zero; // Якщо цілі немає або не атакує — стоїмо
        }
    }
}
