using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TouchUI : MonoBehaviour
{
    public Button leftButton;
    public Button rightButton;
    public Button jumpButton;
    public Button throwButton;
    public Button exitButton;

    public Control playerControl;
    public GameMenus gameMenu;

    //     void Update()
    // {
    //     Debug.Log("Touch count: " + Input.touchCount);
    // }

    void Start()
    {
        // Додаємо обробники для натискань та відпускань
        AddEvent(leftButton, EventTriggerType.PointerDown, (e) => playerControl.TouchLeft());
        AddEvent(leftButton, EventTriggerType.PointerUp, (e) => playerControl.UnTouchLeft());

        AddEvent(rightButton, EventTriggerType.PointerDown, (e) => playerControl.TouchRight());
        AddEvent(rightButton, EventTriggerType.PointerUp, (e) => playerControl.UnTouchRight());

        // Стрибок — при натисканні
        AddEvent(jumpButton, EventTriggerType.PointerDown, (e) => playerControl.TouchJump());

        // Кидок — при натисканні
        AddEvent(throwButton, EventTriggerType.PointerDown, (e) => playerControl.TouchThrow());

        AddEvent(exitButton, EventTriggerType.PointerDown, (e) => gameMenu.TouchExit());
    }

    void AddEvent(Button button, EventTriggerType eventType, System.Action<BaseEventData> action)
    {
        EventTrigger trigger = button.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }
}
