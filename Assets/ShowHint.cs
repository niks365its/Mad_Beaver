using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowHint : MonoBehaviour
{
    public GameObject hint;
    private string hintKey;

    void Start()
    {
        Debug.Log("Скрипт ShowHint прикріплений до: " + gameObject.name);
        // Створюємо унікальний ключ для кожної сцени
        string sceneName = SceneManager.GetActiveScene().name;
        hintKey = $"ImageShown_{sceneName}";

        // Перевіряємо, чи вже показували зображення для цієї сцени
        if (PlayerPrefs.GetInt(hintKey, 0) == 1)
        {
            hint.SetActive(false); // Вимикаємо об'єкт
        }
        else
        {
            StartCoroutine(HintShow());
        }
    }

    IEnumerator HintShow()
    {
        yield return new WaitForSeconds(2f); // Затримка перед показом
        hint.SetActive(true);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.hintSound);

        // Позначаємо, що зображення було показано для цієї сцени
        PlayerPrefs.SetInt(hintKey, 1);
        PlayerPrefs.Save();
    }

    // Скидаємо підсказки для всіх сцен при виході (не обов'язково)
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll(); // Якщо хочеш обнуляти всі підсказки
    }
}
