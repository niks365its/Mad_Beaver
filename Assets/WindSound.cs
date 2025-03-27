using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindSound : MonoBehaviour
{
    private bool onWind;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !onWind)
        {
            onWind = true;
            SoundManager.Instance.PlayBackgroundMusic(SoundManager.Instance.windSound);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && onWind)
        {
            onWind = false;
            SoundManager.Instance.PlayBackgroundMusic(SoundManager.Instance.backgroundSound);
        }
    }
}
