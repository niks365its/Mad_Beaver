using UnityEngine;

public class DeathZoneHandler : MonoBehaviour
{

    public HealthBar healthBar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Перевірка, чи це персонаж
        {
            Control player = collision.GetComponent<Control>();
            if (player != null)
            {
                //  player.TriggerGameOver(); // Виклик анімації програшу

                //GetComponent<Control>().enabled = false; // Зупиняємо рух або інші дії персонажа
                healthBar.ZeroHealth();


            }
        }
    }



}
