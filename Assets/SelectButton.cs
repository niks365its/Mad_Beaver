using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour
{
    private EventSystem eventSystem;
    private void OnEnable()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(gameObject);
        }
    }
}
