using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

            if (info.sceneIndex > reachedSceneIndex)
            {
                if (topImage != null) topImage.SetActive(true);

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

                // Відключення EventTrigger
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
