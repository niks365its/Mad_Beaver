using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameMenus : MonoBehaviour
{
    private Input menuInput;
    public GameObject exitMenu;
    public GameObject Player;
    public GameObject LevelMenu;


    public void Awake()
    {
        menuInput = new Input();

        menuInput.player.Exit.performed += onExitMenu;
    }
    // Метод для кнопки "Почати гру"
    public void GameStart()
    {
        // Завантажуємо сцену з грою (замініть "GameScene" на назву вашої сцени)
        StartCoroutine(SceneLoader());
    }

    private IEnumerator SceneLoader()
    {
        yield return new WaitForSeconds(1); // Затримка на 1 секунду перед завантаженням сцени
        SceneManager.LoadScene(1); // Завантажуємо сцену з грою (замініть "GameScene" на назву вашо�� сцени)

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

    public void BackToMaim()
    {
        LevelMenu.SetActive(false);

    }

    public void LevelSelect()
    {
        StartCoroutine(LevelSelectDelay()); // Викликаємо корутину затримки перед закриттям
    }

    private IEnumerator LevelSelectDelay()
    {
        yield return new WaitForSeconds(1); // Затримка на 1 секунди перед закриттям

        LevelMenu.SetActive(true);
    }

    // Метод для кнопки "Вихід з гри"
    public void ExitGame()
    {
        StartCoroutine(ExitDelay()); // Викликаємо корутину затримки перед закриттям
    }

    private IEnumerator ExitDelay()
    {
        yield return new WaitForSeconds(1); // Затримка на 1 секунди перед закриттям
                                            // Закриваємо гру (працює тільки у збірці)
        Application.Quit();
        // Для редактора:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}