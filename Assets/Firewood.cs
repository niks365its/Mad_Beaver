using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Firewood : MonoBehaviour
{
    public Text firewoodText;
    public int addSum = 10;

    public AudioSource audioSource;
    public AudioClip woodAddSound;

    private void Start()
    {
        firewoodText.text = "" + GlobalResources.Firewood;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GlobalResources.Firewood += addSum;
            firewoodText.text = "" + GlobalResources.Firewood;
            audioSource.PlayOneShot(woodAddSound);
            Destroy(gameObject, woodAddSound.length);
        }
    }
}


