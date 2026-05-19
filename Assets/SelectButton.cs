using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectButton : MonoBehaviour
{
    private EventSystem eventSystem;

    private void OnEnable()
    {
        StartCoroutine(SelectWithDelay());
    }

    private IEnumerator SelectWithDelay()
    {
        yield return new WaitForSeconds(0.2f);

        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(gameObject);
        }
    }
}