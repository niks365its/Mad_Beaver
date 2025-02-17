using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sharp : MonoBehaviour
{
    public Animator animator;
    public HealthBar healthBar;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Control player = collision.GetComponent<Control>();
            animator.SetBool("IsJump", false);
            // animator.SetTrigger("GameOverTrigger");
            //  audioSource.PlayOneShot(sharpHitSound);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.sharpSound);
            player.enabled = false;
            healthBar.ZeroHealth();
        }
    }
}
