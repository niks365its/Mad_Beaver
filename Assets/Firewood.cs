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

    //  public GameObject SticksInBag;

    public float speed = 5f;
    public float rotationSpeed = 100f;

    public float stopDistance = 0.05f;

    private bool isFollowing = false;

    private bool isCollected = false;

    private void Start()
    {
        firewoodText.text = "" + GlobalResources.Firewood;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected || !other.CompareTag("Player"))
            return;

        isCollected = true;

        if (hintAnimator != null)
        {
            hintAnimator.SetBool("IsHintExit", true);
        }
        StartCoroutine(FollowTarget());
        GlobalResources.Firewood += addSum;
        firewoodText.text = "" + GlobalResources.Firewood;
        //audioSource.PlayOneShot(woodAddSound);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.woodGetSound);
        Destroy(gameObject, SoundManager.Instance.woodGetSound.length);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected || !other.CompareTag("Player"))
            return;

        isCollected = true;

        if (hintAnimator != null)
        {
            hintAnimator.SetBool("IsHintExit", true);
        }
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

            Vector3 rotation = objectToMove.eulerAngles;
            rotation.x = Mathf.MoveTowardsAngle(rotation.x, -80f, rotationSpeed * Time.deltaTime);
            objectToMove.eulerAngles = rotation;

            yield return null;
        }
        // SticksInBag.SetActive(true);
        isFollowing = false;
        Destroy(gameObject);    //, SoundManager.Instance.woodGetSound.length);
    }
}


