using UnityEngine;

public class TurretFollow : MonoBehaviour
{
    public Transform target; // Посилання на персонажа
    public Transform hunter; // Посилання на мисливця

    void Update()
    {
        if (target != null && hunter != null)
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

            float angle = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg; // Кут повороту
            transform.rotation = Quaternion.Euler(0, 0, angle); // Застосовуємо поворот
        }
    }
}