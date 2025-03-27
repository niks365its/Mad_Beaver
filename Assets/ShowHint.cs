using System.Collections;
using UnityEngine;

public class ShowHint : MonoBehaviour
{
    public GameObject hint;
    void Start()
    {
        // Перевіряємо, чи вже показували зображення в цій сесії
        if (PlayerPrefs.GetInt("ImageShown", 0) == 1)
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
        yield return new WaitForSeconds(2f); // Чекаємо перед появою
        hint.SetActive(true);  //  з'являється
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.hintSound);



        // Позначаємо, що зображення було показано
        PlayerPrefs.SetInt("ImageShown", 1);
        PlayerPrefs.Save();
    }



    // Скидаємо "ImageShown" при виході з гри
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("ImageShown");
    }
}