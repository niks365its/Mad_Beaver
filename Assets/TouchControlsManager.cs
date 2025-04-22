using UnityEngine;
using UnityEngine.InputSystem;

public class TouchControlsManager : MonoBehaviour
{
    public static TouchControlsManager Instance { get; private set; }

    [Header("UI для тач-екранів")]
    [SerializeField] private GameObject touchControlsUI;

    /// <summary>
    /// Повертає true, якщо це пристрій із сенсорним екраном і не Android TV.
    /// </summary>
    public bool IsTouch { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // опціонально
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        IsTouch = IsTouchDevice() && !IsAndroidTV();
    }

    private void Start()
    {
        if (touchControlsUI != null)
        {
            touchControlsUI.SetActive(IsTouch);
        }

        Debug.Log($"[TouchControlsManager] IsTouch = {IsTouch}");
    }

    /// <summary>
    /// Перевірка наявності сенсорного екрану.
    /// </summary>
    private bool IsTouchDevice()
    {
        foreach (var device in InputSystem.devices)
        {
            if (device is Touchscreen)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Перевірка, чи це Android TV.
    /// </summary>
    private bool IsAndroidTV()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");

                // Перевірка системних налаштувань
                using (var settings = new AndroidJavaClass("android.provider.Settings$Global"))
                {
                    string deviceName = settings.CallStatic<string>("getString", contentResolver, "device_name");
                    if (!string.IsNullOrEmpty(deviceName) && deviceName.ToLower().Contains("tv"))
                        return true;
                }

                // Перевірка моделі пристрою
                using (var build = new AndroidJavaClass("android.os.Build"))
                {
                    string model = build.GetStatic<string>("MODEL");
                    string product = build.GetStatic<string>("PRODUCT");

                    if ((model != null && model.ToLower().Contains("tv")) ||
                        (product != null && product.ToLower().Contains("tv")))
                        return true;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[TouchControlsManager] Android TV check failed: " + e.Message);
        }
#endif
        return false;
    }
}
