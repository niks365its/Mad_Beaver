using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigationSound : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip hoverSound;
    private GameObject lastSelected;

    void Start()
    {
        // Додаємо AudioSource до об'єкта
        audioSource = gameObject.AddComponent<AudioSource>();

        // Завантажуємо звук із вбудованих ресурсів (повинен бути в папці "Resources")
        hoverSound = Resources.Load<AudioClip>("Sounds/pop-39222");

        if (hoverSound == null)
        {
            Debug.LogError("Не знайдено звуковий файл hover_sound у папці Resources!");
        }
    }

    void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != lastSelected)
        {
            if (hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
            lastSelected = currentSelected;
        }
    }
}
