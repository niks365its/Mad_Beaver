using UnityEngine;
using UnityEngine.EventSystems;

public class MenuNavigationSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverSound;

    private GameObject lastSelected;

    void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != lastSelected)
        {
            audioSource.PlayOneShot(hoverSound);
            lastSelected = currentSelected;
        }
    }
}
