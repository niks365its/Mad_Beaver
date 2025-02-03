using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovedBalk : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform); // Призначаємо батьком саме `EmptyParent`
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Викликаємо зміну батька після того, як об'єкт залишить зіткнення
            StartCoroutine(DetachParent(collision.transform));
        }
    }

    private IEnumerator DetachParent(Transform playerTransform)
    {
        yield return null; // Очікуємо один кадр, щоб уникнути зміни батька під час активації/деактивації
        playerTransform.SetParent(null);
    }

}
