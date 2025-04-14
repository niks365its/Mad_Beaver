using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sharp : MonoBehaviour
{
    public Animator animator;
    public HealthBar healthBeaver;


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Control player = collision.GetComponent<Control>();
            animator.SetBool("IsJump", false);

            animator.SetTrigger("GameOverTrigger");
            animator.SetBool("IsDead", true);

            //  audioSource.PlayOneShot(sharpHitSound);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.sharpSound);
            player.enabled = false;
            healthBeaver.ZeroHealth();
        }
    }
}