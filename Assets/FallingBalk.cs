using UnityEngine;

public class FallingBalk : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Перевіряємо, чи зіткнулась балка з гравцем
        {
            Invoke("EnableGravity", 3f); // Запускаємо затримку перед увімкненням гравітації
        }
    }

    private void EnableGravity()
    {
        Debug.Log("IsContact!!!");
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f; // Включаємо гравітуцію

    }
}
