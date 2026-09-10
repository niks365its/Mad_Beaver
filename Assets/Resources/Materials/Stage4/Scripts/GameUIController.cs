
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameUIController : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverMenu;
    public GameObject screenController;
    public Text firewoodText;

    [Header("Player")]
    public GameObject Player;
    // public Control playerControl;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject subMenu1;
    public GameObject subMenu2;
    public GameObject subMenu3;

    [Header("Menu First Buttons")]
    public Button mainMenuFirstButton;
    public Button subMenu1FirstButton;
    public Button subMenu2FirstButton;
    public Button subMenu3FirstButton;

    private bool isGameOver = false;

    private void Start()
    {
        UpdateFirewoodText();

        if (TouchControlsManager.Instance != null)
        {
            screenController.SetActive(TouchControlsManager.Instance.IsTouch);
        }
        else
        {
            screenController.SetActive(false);
        }
    }

    private void Update()
    {
        // Якщо вже є вибраний об'єкт, нічого не змінюємо
        if (EventSystem.current.currentSelectedGameObject != null)
            return;

        // Якщо миша наведена на кнопку, нічого не змінюємо
        if (IsPointerOverUIElement())
            return;

        // Визначаємо, яке меню активне
        if (IsMenuActive(subMenu3, subMenu3FirstButton))
            return;

        if (IsMenuActive(subMenu2, subMenu2FirstButton))
            return;

        if (IsMenuActive(subMenu1, subMenu1FirstButton))
            return;

        if (IsMenuActive(mainMenu, mainMenuFirstButton))
            return;
    }

    // =========================
    // FIREWOOD UI
    // =========================

    public void UpdateFirewoodText()
    {
        if (firewoodText != null)
            firewoodText.text = GlobalResources.Firewood.ToString();
    }

    public void ShowFirewoodEmpty()
    {
        if (firewoodText != null)
            firewoodText.text = "X";
    }

    // =========================
    // GAME OVER
    // =========================

    public void TriggerGameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (screenController != null)
            screenController.SetActive(false);

        StartCoroutine(GameOverMenu());
    }

    private IEnumerator GameOverMenu()
    {
        yield return new WaitForSeconds(3f);

        SoundManager.Instance.StopEffectsSound();
        SoundManager.Instance.StopBackgroundSound();

        if (gameOverMenu != null)
            gameOverMenu.SetActive(true);

        SoundManager.Instance.PlayOneShot(SoundManager.Instance.gameOverSound);

        if (Player != null)
            Player.SetActive(false);
    }

    // =========================
    // SCENE / GAME
    // =========================

    public void RestartGame()
    {

        if (SoundManager.Instance != null)
            SoundManager.Instance.StopEffectsSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Scene restart " + SceneManager.GetActiveScene().name);
    }

    public void NextLevel(int level)
    {
        SoundManager.Instance.StopEffectsSound();

        SceneManager.LoadScene(level);

        GameMenus gameMenus = FindObjectOfType<GameMenus>();

        if (gameMenus != null)
            gameMenus.LevelCompleted(level);
    }

    public void ExitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // =========================
    // TOUCH CONTROLS
    // =========================

    // public void TouchLeft()
    // {
    //     if (playerControl != null)
    //         playerControl.TouchLeft();
    // }

    // public void TouchRight()
    // {
    //     if (playerControl != null)
    //         playerControl.TouchRight();
    // }

    // public void UnTouchLeft()
    // {
    //     if (playerControl != null)
    //         playerControl.UnTouchLeft();
    // }

    // public void UnTouchRight()
    // {
    //     if (playerControl != null)
    //         playerControl.UnTouchRight();
    // }

    // public void TouchJump()
    // {
    //     if (playerControl != null)
    //         playerControl.TouchJump();
    // }

    // public void TouchThrow()
    // {
    //     if (playerControl != null)
    //         playerControl.TouchThrow();
    // }

    public void SetTouchControls(bool state)
    {
        if (screenController != null)
            screenController.SetActive(state);
    }

    // =========================
    // MENU NAVIGATION
    // =========================

    private bool IsMenuActive(GameObject menu, Button firstButton)
    {
        if (menu != null && menu.activeSelf && firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
            return true;
        }

        return false;
    }

    private bool IsPointerOverUIElement()
    {
        if (Mouse.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(eventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
                return true;
        }

        return false;
    }
}
