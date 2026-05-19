using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelMenus : MonoBehaviour
{
    public Button[] levelButtons; // Призначити у інспекторі
    private int reachedSceneIndex;

    private void Start()
    {
        reachedSceneIndex = PlayerPrefs.GetInt("ReachedScene", 1);
        UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        foreach (Button btn in levelButtons)
        {
            LevelButtonInfo info = btn.GetComponent<LevelButtonInfo>();
            if (info == null)
            {
                Debug.LogWarning("LevelButtonInfo не знайдено на кнопці: " + btn.name);
                continue;
            }

            GameObject topImage = btn.transform.Find("topImage")?.gameObject;
            // Пошук TMP тексту "LevelName"
            TMP_Text levelNameText = btn.transform.Find("LevelName")?.GetComponent<TMP_Text>();

            if (info.sceneIndex > reachedSceneIndex)
            {
                if (topImage != null) topImage.SetActive(true);

                // Прозорість тексту 30%
                if (levelNameText != null)
                {
                    Color color = levelNameText.color;
                    color.a = 0.3f;
                    levelNameText.color = color;
                }

                // Відключення EventTrigger
                EventTrigger eventTrigger = btn.GetComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    eventTrigger.enabled = false;
                }

                btn.interactable = false;
            }
            else
            {
                if (topImage != null) topImage.SetActive(false);

                // Повернення нормальної прозорості
                if (levelNameText != null)
                {
                    Color color = levelNameText.color;
                    color.a = 1f;
                    levelNameText.color = color;
                }

                // Включення EventTrigger
                EventTrigger eventTrigger = btn.GetComponent<EventTrigger>();
                if (eventTrigger != null)
                {
                    eventTrigger.enabled = true;
                }

                btn.interactable = true;
            }
        }
    }
}
