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
    private GameObject player;
    private Control control;
    private Control3 control3;
    void Start()
    {
        Debug.Log("Скрипт HealthBar прикріплений до: " + gameObject.name);


        currentHealth = maxHealth; // Задати початкове здоров'я
        healthSlider.maxValue = maxHealth; // Налаштувати слайдер
        healthSlider.value = currentHealth;

        // Відобразити початкову кількість життів
        UpdateLifeText();

        // Знайти об'єкт гравця
        player = GameObject.Find("BoberTop");

        if (player == null)
        {
            Debug.LogError("Об'єкт Player не знайдено в сцені!");
            return;
        }

        control = player.GetComponent<Control>();
        control3 = player.GetComponent<Control3>();
        if (control == null && control3 == null)
        {
            Debug.LogError("На Player немає ні Control, ні Control3!");
        }
    }

    public void IncreaseLife()
    {
        life++; // Збільшити життя на 1
        UpdateLifeText(); // Оновити відображення життів
    }

    public void ZeroHealth()
    {


        animator.SetBool("IsDead", true);

        if (life > 1)
        {
            StartCoroutine(HandleGameOver());

            if (control != null)
            {
                control.enabled = false;
            }

            if (control3 != null)
            {
                control3.enabled = false;
            }
        }

        else
        {

            Die(); // Виклик логіки смерті

        }
        Debug.Log("healthSlider" + currentHealth);
    }

    IEnumerator HandleGameOver()
    {


        animator.SetBool("IsDead", true); // Викликаємо анімацію
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
            animator.SetBool("IsJump", false);


            animator.SetBool("IsDead", true);

            SoundManager.Instance.PlayOneShot(SoundManager.Instance.deathSound);

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

        if (control != null)
        {
            control.TriggerGameOver();
        }

        if (control3 != null)
        {
            control3.TriggerGameOver();
        }
    }
}
