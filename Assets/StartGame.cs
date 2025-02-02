using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Метод для кнопки "Почати гру"
    public void GameStart()
    {
        // Завантажуємо сцену з грою (замініть "GameScene" на назву вашої сцени)
        SceneManager.LoadScene(1);
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