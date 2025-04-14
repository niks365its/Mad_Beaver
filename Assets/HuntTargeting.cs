using UnityEngine;

public class TurretFollow : MonoBehaviour
{
    public Transform target; // Посилання на персонажа
    public Transform hunter; // Посилання на мисливця
    public float speed = 2f; // Швидкість переміщення по Y
    public bool isPike = false;

    void Update()
    {
        HunterControl hunterScript = hunter.GetComponent<HunterControl>();
        if (target != null && hunter != null && hunterScript.IsAttacking)
        {
            Vector3 direction = target.position - transform.position; // Вектор до цілі

            // Перевіряємо, чи персонаж знаходиться праворуч від мисливця
            if (target.position.x > hunter.position.x)
            {
                direction.y = -direction.y;
                direction.x = -direction.x;
            }

            // Перевірка умови для скидання обертання
            if ((hunter.localScale.x == 1 && target.position.x < hunter.position.x) ||
                (hunter.localScale.x == -1 && target.position.x > hunter.position.x))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                return;
            }

            if (!isPike)
            {
                float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; // Кут повороту
                transform.rotation = Quaternion.Euler(0, 0, angle); // Застосовуємо поворот
            }

            else
            {
                // Цільова позиція по Y — позиція персонажа
                float targetY = target.position.y + 1f;
                float currentY = transform.position.y;

                // Плавне переміщення по Y до цілі
                float newY = Mathf.MoveTowards(currentY, targetY, speed * Time.deltaTime);

                // Оновлюємо позицію (залишаючи X і Z без змін)
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
        }
    }
}
