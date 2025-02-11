using UnityEngine;

public class Mushroom : MonoBehaviour
{

    public HealthBar healthBar;
    private string mushroomKey;

    public AudioSource audioSource;
    public AudioClip lifeAddSound;

    private void Start()
    {

        // Унікальний ключ для збереження стану грибка
        mushroomKey = "Mushroom_" + gameObject.name;

        if (PlayerPrefs.HasKey(mushroomKey))
        {
            // Якщо грибок вже був зібраний, видаляємо його
            if (PlayerPrefs.GetInt(mushroomKey, 0) == 1)
            {
                Destroy(gameObject);

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Перевірка, чи це персонаж
        {
            Control player = collision.GetComponent<Control>();
            if (player != null)
            {
                //  player.TriggerGameOver(); // Виклик анімації програшу

                //GetComponent<Control>().enabled = false; // Зупиняємо рух або інші дії персонажа
                healthBar.IncreaseLife();
                // Вимикаємо візуальне відображення об'єкта
                GetComponent<SpriteRenderer>().enabled = false;
                PlayerPrefs.SetInt(mushroomKey, 1); // Позначаємо грибок як зібраний
                PlayerPrefs.Save(); // Зберігаємо стан
                //Destroy(gameObject); // Видаляємо зі сцени грибок
                audioSource.PlayOneShot(lifeAddSound);
                Destroy(gameObject, lifeAddSound.length);

            }
        }
    }
}
