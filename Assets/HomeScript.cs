using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScript : MonoBehaviour
{
    public GameObject levelCompleted;
    public GameObject Player;
    public GameObject home;
    public GameObject playerBar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Control player = collision.GetComponent<Control>();
            if (player != null)
            {
                home.SetActive(false);
                levelCompleted.SetActive(true);

                int currentLevel = SceneManager.GetActiveScene().buildIndex;
                FindObjectOfType<GameMenus>().LevelCompleted(currentLevel);

                Player.SetActive(false);
                playerBar.SetActive(false);
                SoundManager.Instance.PlayOneShot(SoundManager.Instance.fireworkSound);
            }
        }
    }
}
