using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    public GameObject levelCompleted;
    public GameObject Player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Перевірка, чи це персонаж
        {
            Control player = collision.GetComponent<Control>();
            if (player != null)
            {
                levelCompleted.SetActive(true);
                Player.SetActive(false);


            }
        }
    }
}