using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Посилання на слайдер
    public float maxHealth = 100f; // Максимальне здоров'я ворога
    private float currentHealth;
    private bool isHealthBarVisible = false; // Чи активна смуга
    public Animator enemyAnimator;
    public bool deadIsAnimated = false;
    private bool isStarted = false;


    void Start()
    {
isStarted = true;
        healthSlider.gameObject.SetActive(false); // Вимкнути смугу на старті
    }

    public void TakeDamage(float damage)
    {
        if (isStarted)
        {
                currentHealth = maxHealth; // Задати початкове здоров'я
       healthSlider.maxValue = maxHealth; // Налаштувати слайдер
       Debug.Log("Health Max is " + maxHealth);
        healthSlider.value = currentHealth;
        }

        Debug.Log("Health before " + currentHealth);
        currentHealth -= damage; // Зменшити здоров'я
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Обмежити діапазон
        healthSlider.value = currentHealth; // Оновити смугу життя
         
        Debug.Log("Health after " + currentHealth);
        
        if (!isHealthBarVisible)
        {
            ShowHealthBar(); // Показати смугу життя після першого пошкодження
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(DeathAnimation()); // Викликати анімацію смерті
        }
        isStarted = false;
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

    public IEnumerator DeathAnimation()
    {
        // yield return new WaitForSeconds(1f);
        enemyAnimator.SetBool("IsDead", true);
        yield return new WaitForSeconds(0.3f);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.sharpSound);

        if (deadIsAnimated)
        {
            yield return new WaitForSeconds(0.5f);
            enemyAnimator.SetBool("IsDead", false);
        }

        Die(); // Викликати логіку смерті
    }
}
