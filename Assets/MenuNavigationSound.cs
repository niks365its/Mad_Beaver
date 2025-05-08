using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigationSound : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip hoverSound;
    private GameObject lastSelected;

    private void Start()
    {

        Debug.Log($"Script is attached to: {gameObject.name}", gameObject);

        // Отримуємо AudioSource компонента
        audioSource = GetComponent<AudioSource>();

        if (hoverSound == null)
        {
            Debug.LogError("Не знайдено звуковий файл hover_sound!");
        }
    }

    private void Update()
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

    // Реалізація методу для обробки події наведеного курсору
    public void OnPointerEnter()
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
}
