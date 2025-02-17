using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuNavigation : MonoBehaviour
{
    public Button defaultButton;  // Кнопка за замовчуванням
    private Button currentSelectedButton;

    private void Start()
    {
        // Спочатку активуємо кнопку за замовчуванням
        currentSelectedButton = defaultButton;
        currentSelectedButton.Select();  // Вибирає кнопку
    }

    // Викликається при наведенні на кнопку мишкою
    public void OnMouseOverButton(Button button)
    {
        if (currentSelectedButton != null)
        {
            // Деактивуємо поточну вибрану кнопку
            currentSelectedButton.OnDeselect(null);
        }

        // Активуємо нову кнопку, на яку наведено мишку
        currentSelectedButton = button;
    }

    // Викликається, коли натискається кнопка
    public void OnButtonClick()
    {
        // Після натискання на кнопку активуємо кнопку за замовчуванням
        currentSelectedButton = defaultButton;
        currentSelectedButton.Select();
    }
}