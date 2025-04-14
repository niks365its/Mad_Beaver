using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class GameMenus : MonoBehaviour
{
    private Input menuInput;
    public GameObject exitMenu;
    public GameObject Player;
    public GameObject LevelMenu;

    private int selectedSceneIndex;
    private int reachedSceneIndex;
    private Button selectedButton = null; // Поточна вибрана кнопка
    private EventSystem eventSystem;
    public GameObject targetButton;

    public void Awake()
    {
        menuInput = new Input();

        menuInput.player.Exit.performed += onExitMenu;

        // Отримуємо останній досягнутий рівень
        reachedSceneIndex = PlayerPrefs.GetInt("ReachedScene", 1);

        // Отримуємо вибраний рівень, якщо він є (інакше використовуємо досягнутий)
        if (PlayerPrefs.HasKey("SelectedScene"))
        {
            // Якщо ключ "SelectedScene" існує в PlayerPrefs, отримуємо збережене значення
            selectedSceneIndex = PlayerPrefs.GetInt("SelectedScene");
        }
        else
        {
            // Якщо ключа немає, використовуємо значення досягнутого рівня (ReachedScene)
            selectedSceneIndex = reachedSceneIndex;
        }
    }


    public void SelectLevel(int sceneIndex)
    {
        // Користувач вибирає рівень у меню
        selectedSceneIndex = sceneIndex;
        PlayerPrefs.SetInt("SelectedScene", sceneIndex);
        PlayerPrefs.Save();
    }

    public void LevelCompleted(int nextSceneIndex)
    {
        // Оновлюємо досягнутий рівень, якщо він більший за збережений
        if (nextSceneIndex > reachedSceneIndex)
        {
            reachedSceneIndex = nextSceneIndex;
            PlayerPrefs.SetInt("ReachedScene", reachedSceneIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetSelectedButton(Button button)
    {
        // Змінюємо стан вибраної кнопки
        if (selectedButton != null)
        {
            selectedButton.interactable = true; // Активуємо попередню кнопку
        }

        selectedButton = button;
        selectedButton.interactable = false; // Деактивуємо поточну кнопку
        eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.SetSelectedGameObject(targetButton);
    }

    public void GameStart()
    {

        SceneManager.LoadScene(selectedSceneIndex);
    }


    // private IEnumerator SceneLoader(int sceneIndex)
    // {
    //     yield return new WaitForSeconds(1); // Затримка на 1 секунду перед завантаженням сцени
    //     SceneManager.LoadScene(sceneIndex); // Завантажуємо сцену з грою (замініть "GameScene" на назву вашо�� сцени)

    // }
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

    public void TouchExit()
    {
        var fakeContext = new InputAction.CallbackContext();
        onExitMenu(fakeContext);
    }
}