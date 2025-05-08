using UnityEditor;
using UnityEngine;

public static class PlayerPrefsTools
{
    [MenuItem("Tools/Clear PlayerPrefs %#d")] // Ctrl+Shift+D (Windows), Cmd+Shift+D (macOS)
    public static void ClearPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Очистити PlayerPrefs?",
            "Всі локальні збереження будуть видалені. Продовжити?", "Так", "Скасувати"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("✅ PlayerPrefs очищено.");
        }
    }
}
