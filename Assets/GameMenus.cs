using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameMenus : MonoBehaviour
{
    private Input menuInput;
    public GameObject exitMenu;
    public GameObject Player;


    public void Awake()
    {
        menuInput = new Input();

        menuInput.player.Exit.performed += onExitMenu;
    }
    // Метод для кнопки "Почати гру"
    public void GameStart()
    {
        // Завантажуємо сцену з грою (замініть "GameScene" на назву вашої сцени)
        SceneManager.LoadScene(1);
    }
    private void OnEnable()
    {
        menuInput.Enable();
    }

    private void OnDisable()
    {
        menuInput.Disable();
    }

    public void MainMenu()
    {
        // Завантажуємо сцену з грою (замініть "GameScene" на назву вашої сцени)
        SceneManager.LoadScene(0);
    }

    public void onExitMenu(InputAction.CallbackContext context)
    {
        Debug.Log("onExitMenu is " + exitMenu.activeSelf);
        if (!exitMenu.activeSelf)
        {
            exitMenu.SetActive(true);
            Player.SetActive(false);
        }
        else
        {
            exitMenu.SetActive(false);
            Player.SetActive(true);
        }
    }


    public void Resume()
    {
        exitMenu.SetActive(false);
        Player.SetActive(true);
    }

    // Метод для кнопки "Вихід з гри"
    public void ExitGame()
    {
        // Закриваємо гру (працює тільки у збірці)
        Application.Quit();
        // Для редактора:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}