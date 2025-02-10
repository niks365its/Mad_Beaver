using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    public Animator animator;
    public Slider healthSlider; // Посилання на смугу життя
    public Text lifeText; // Текст для відображення кількості життів
    public float maxHealth = 100f; // Максимальне здоров'я
    private float currentHealth;
    public static int life = 3;

    // Посилання на об'єкт гравця
    private Control player;

    void Start()
    {
        Debug.Log("Скрипт прикріплений до: " + gameObject.name);


        currentHealth = maxHealth; // Задати початкове здоров'я
        healthSlider.maxValue = maxHealth; // Налаштувати слайдер
        healthSlider.value = currentHealth;

        // Відобразити початкову кількість життів
        UpdateLifeText();

        // Знайти об'єкт гравця
        player = FindObjectOfType<Control>();
        if (player == null)
        {
            Debug.LogError("PlayerController не знайдено в сцені!");
        }
    }

    public void IncreaseLife()
    {
        life++; // Збільшити життя на 1
        UpdateLifeText(); // Оновити відображення життів
    }

    public void ZeroHealth()
    {
        if (life > 1)
        {
            StartCoroutine(HandleGameOver());
            player.enabled = false;
        }

        else
        {

            Die(); // Виклик логіки смерті

        }
        Debug.Log("healthSlider" + currentHealth);
    }

    IEnumerator HandleGameOver()
    {
        animator.SetTrigger("GameOverTrigger"); // Викликаємо анімацію
        yield return new WaitForSeconds(3f); // Чекаємо 3 секунди
        life--; // Зменшуємо кількість життя
        UpdateLifeText(); // Оновлюємо текст життя
        RestartLevel(); // Перезапускаємо рівень
        GlobalResources.Firewood = 0;
    }


    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Зменшити здоров'я
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Обмежити діапазон
        healthSlider.value = currentHealth; // Оновити смугу життя

        if (currentHealth <= 0)
        {
            ZeroHealth();

        }


    }

    private void UpdateLifeText()
    {
        if (lifeText != null)
        {
            lifeText.text = "" + life; // Оновити текст
        }
    }

    private void RestartLevel()
    {
        currentHealth = maxHealth; // Відновити здоров'я
        healthSlider.value = currentHealth; // Оновити смугу життя
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Перезапустити поточний рівень
    }

    private void Die()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        GlobalResources.Firewood = 0;

        if (player != null)
        {
            player.TriggerGameOver(); // Виклик логіки програшу
        }

    }
}
