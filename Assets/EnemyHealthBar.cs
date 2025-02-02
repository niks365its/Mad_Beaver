using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Посилання на слайдер
    public float maxHealth = 50f; // Максимальне здоров'я ворога
    private float currentHealth;
    private bool isHealthBarVisible = false; // Чи активна смуга

    void Start()
    {
        currentHealth = maxHealth; // Задати початкове здоров'я
        healthSlider.maxValue = maxHealth; // Налаштувати слайдер
        healthSlider.value = currentHealth;
        healthSlider.gameObject.SetActive(false); // Вимкнути смугу на старті
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage; // Зменшити здоров'я
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Обмежити діапазон
        healthSlider.value = currentHealth; // Оновити смугу життя

        if (!isHealthBarVisible)
        {
            ShowHealthBar(); // Показати смугу життя після першого пошкодження
        }

        if (currentHealth <= 0)
        {
            Die(); // Викликати логіку смерті

        }
    }

    private void ShowHealthBar()
    {
        healthSlider.gameObject.SetActive(true); // Увімкнути слайдер
        isHealthBarVisible = true;

    }

    private void Die()
    {
        Debug.Log("Ворог загинув!");
        Destroy(gameObject); // Видалити ворога зі сцени
        healthSlider.gameObject.SetActive(false);
    }
}
