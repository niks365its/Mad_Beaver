using UnityEngine;

public class DeathZoneHandler : MonoBehaviour
{

    public HealthBar healthBar;

    // public AudioSource audioSource;
    // public AudioClip fallingSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Перевірка, чи це персонаж
        {
            // audioSource.PlayOneShot(fallingSound);

            SoundManager.Instance.PlayOneShot(SoundManager.Instance.fallSound);



            Control player = collision.GetComponent<Control>();
            if (player != null)
            {
                //  player.TriggerGameOver(); // Виклик анімації програшу

                player.enabled = false; // Зупиняємо рух або інші дії персонажа
                healthBar.ZeroHealth();


            }
        }
    }



}
