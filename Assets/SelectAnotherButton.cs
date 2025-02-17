using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectAnotherButton : MonoBehaviour
{
    public GameObject targetObject; // Об'єкт, який буде вибрано

    private EventSystem eventSystem;

    public void SelectObj()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null && targetObject != null)
        {
            eventSystem.SetSelectedGameObject(targetObject);
        }
        else
        {
            Debug.LogWarning("EventSystem або targetObject не знайдено!");
        }
    }
}
