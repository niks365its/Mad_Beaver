using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovedBalk : MonoBehaviour

{
    private Animator animator;
    private float startX, startZ;

    void Start()
    {

        startX = transform.position.x;
        startZ = transform.position.z;

        animator = GetComponent<Animator>();
        float randomDelay = Random.Range(0f, 2f); // Випадкове значення 0-2 секунди
        animator.Play("MovedBalk", 0, randomDelay);


    }

    void LateUpdate()
    {
        transform.position = new Vector3(startX, transform.position.y, startZ);
    }

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
