using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuNavigation : MonoBehaviour
{
    public GameObject mainMenu;          // Головне меню (може бути null)
    public GameObject subMenu1;          // Субменю 1 (може бути null)
    public GameObject subMenu2;          // Субменю 2 (може бути null)
    public GameObject subMenu3;          // Субменю 3 (може бути null)

    public Button mainMenuFirstButton;   // Перша кнопка головного меню (може бути null)
    public Button subMenu1FirstButton;   // Перша кнопка субменю 1 (може бути null)
    public Button subMenu2FirstButton;   // Перша кнопка субменю 2 (може бути null)
    public Button subMenu3FirstButton;   // Перша кнопка субменю 3 (може бути null)

    private void Update()
    {
        // Якщо вже є вибраний об'єкт, нічого не змінюємо
        if (EventSystem.current.currentSelectedGameObject != null) return;

        // Якщо миша наведена на кнопку, нічого не змінюємо
        if (IsPointerOverUIElement()) return;

        // Визначаємо, яке меню активне
        if (IsMenuActive(subMenu3, subMenu3FirstButton)) return;
        if (IsMenuActive(subMenu2, subMenu2FirstButton)) return;
        if (IsMenuActive(subMenu1, subMenu1FirstButton)) return;
        if (IsMenuActive(mainMenu, mainMenuFirstButton)) return;
    }

    private bool IsMenuActive(GameObject menu, Button firstButton)
    {
        if (menu != null && menu.activeSelf && firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            return true; // Меню активне, зупиняємо перевірки
        }
        return false; // Меню неактивне або не вказано
    }

    private bool IsPointerOverUIElement()
    {
        if (Mouse.current == null) return false; // Перевіряємо, чи є миша

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue() // Отримуємо позицію миші
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                return true; // Миша наведена на кнопку
            }
        }
        return false;
    }
}
