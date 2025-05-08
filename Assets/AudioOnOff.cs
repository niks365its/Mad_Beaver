using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class AudioOnOff : MonoBehaviour
{
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    public Image buttonIcon;
    public TMP_Text buttonText;
    public bool isMenu = false;

    private bool isMuted = false;

    private void Start()
    {
        // Завантаження стану звуку
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        ApplyMute();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyMute();

        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyMute()
    {
        AudioListener.pause = isMuted;
        UpdateButtonIcon();
        if (!isMenu)
        {
            SoundManager.Instance.StopWalkSound();
            SoundManager.Instance.StopSound();
        }
    }

    private void UpdateButtonIcon()
    {
        if (buttonIcon != null)
            buttonIcon.sprite = isMuted ? soundOffSprite : soundOnSprite;


        if (buttonText != null)
            buttonText.text = isMuted ? "Off" : "On";
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}
