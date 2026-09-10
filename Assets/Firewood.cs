using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Firewood : MonoBehaviour
{
    public Text firewoodText;
    public int addSum = 10;
    public Animator hintAnimator;
    public Moving script;

    // public AudioSource audioSource;
    // public AudioClip woodAddSound;


    public Transform objectToMove;
    public Transform target;

    public GameObject SticksInBag;

    public float speed = 5f;

    public float stopDistance = 0.05f;

    private bool isFollowing = false;

    private void Start()
    {
        firewoodText.text = "" + GlobalResources.Firewood;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hintAnimator.SetBool("IsHintExit", true);
            StartCoroutine(FollowTarget());
            GlobalResources.Firewood += addSum;
            firewoodText.text = "" + GlobalResources.Firewood;
            //audioSource.PlayOneShot(woodAddSound);
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.woodGetSound);
            Destroy(gameObject, SoundManager.Instance.woodGetSound.length);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // hintAnimator.SetBool("IsHintExit", true);
            StartCoroutine(FollowTarget());
            GlobalResources.Firewood += addSum;
            Debug.Log(" Sticks " + addSum);


            if (script != null)
            {
                script.SetAddSum(addSum);
                Debug.Log(" Sticks send" + addSum);
            }

            firewoodText.text = "" + GlobalResources.Firewood;
            //audioSource.PlayOneShot(woodAddSound);
            //   SoundManager.Instance.PlayOneShot(SoundManager.Instance.woodGetSound);

        }
    }

    private IEnumerator FollowTarget()
    {
        isFollowing = true;

        while (true)
        {
            Vector3 currentTargetPos = target.position;
            Vector3 direction = (currentTargetPos - objectToMove.position).normalized;

            // Якщо вже близько — зупиняємося
            if (Vector3.Distance(objectToMove.position, currentTargetPos) <= stopDistance)
            {
                // objectToMove.position = currentTargetPos;
                Destroy(objectToMove.gameObject);
                break;
            }

            // Рухаємося вперед
            objectToMove.position += direction * speed * Time.deltaTime;

            yield return null;
        }
        SticksInBag.SetActive(true);
        isFollowing = false;
        Destroy(gameObject);    //, SoundManager.Instance.woodGetSound.length);
    }
}


