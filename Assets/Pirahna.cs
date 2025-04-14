using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirahna : MonoBehaviour
{
    private Animator animator;
    public HealthBar healthBar;

    private float startX, startZ;

    void Start()
    {

        startX = transform.position.x;
        startZ = transform.position.z;

        animator = GetComponent<Animator>();
        float randomDelay = Random.Range(0f, 2f); // Випадкове значення 0-2 секунди
        animator.Play("PirahnaIdle", 0, randomDelay);


    }

    void LateUpdate()
    {
        transform.position = new Vector3(startX, transform.position.y, startZ);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Control player = collision.gameObject.GetComponent<Control>();
            animator.SetBool("IsJump", false);

            animator.SetTrigger("GameOverTrigger");
            animator.SetBool("IsDead", true);

            //  audioSource.PlayOneShot(sharpHitSound);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.sharpSound);
            player.enabled = false;
            healthBar.ZeroHealth();
        }
    }
}
