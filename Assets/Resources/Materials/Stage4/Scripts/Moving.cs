
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    [Header("Objects")]
    public GameObject Avto;
    public GameObject AnimDoor;
    public GameObject BaseDoor;
    public Animator animator;
    public Text firewoodText;
    public GameObject stickPrefab; // Префаб камінчика

    public GameObject[] sticks;

    [Header("Parameters")]
    public float speed = 5f;
    public float rotationSpeed = 100f;

    public float jumpForce = 30f;
    public Transform throwPoint;  // Точка, з якої кидатиметься камінчик
    public float throwForce = 10f; // Сила кидка
    public float upForce = 2f;

    private Vector2 beaverMove;
    private Input input;
    private Rigidbody rb;

    private bool isJumping = false;
    private bool isWalking = false;

    private float lastThrowTime = 0f;
    private float throwCooldown = 0.3f;

    private float perOne;
    private float allSum;

    private int addSum;

    private float calcSum;

    public void SetAddSum(int value)
    {
        addSum = value;
        allSum = addSum + calcSum;
        perOne = (float)allSum / sticks.Length;

        Debug.Log("xxx Sticks rest  " + calcSum + " perOne  " + perOne + " allSum  " + allSum);

        for (int i = 0; i < sticks.Length; i++)
        {
            if (sticks[i] != null)
            {
                sticks[i].SetActive(true);
            }
        }
    }

    void Awake()

    {
        input = new Input();

        input.player.CarMove.performed += moveBeaver;
        input.player.CarMove.canceled += moveBeaver;
        input.player.AngleJump.performed += onJump;
        input.player.Throw.performed += stickFly;

        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        Debug.Log(beaverMove);
        Avto.transform.Translate(
            Vector3.forward * -beaverMove.y * speed * Time.deltaTime
        );

        Avto.transform.Rotate(
            Vector3.up * beaverMove.x * rotationSpeed * Time.deltaTime
        );

    }

    private void moveBeaver(InputAction.CallbackContext context)
    {
        beaverMove = context.ReadValue<Vector2>();
        isWalking = beaverMove != Vector2.zero;

        animator.SetBool("IsJump", false);

        animator.SetBool("IsGo", isWalking && beaverMove.y > 0);

        // Рух назад → Mirror
        animator.SetBool("IsGoBack", isWalking && beaverMove.y < 0);
    }

    public void onJump(InputAction.CallbackContext context)
    {

        if (!context.performed)
            return;

        if (!isWalking)
        {
            StartCoroutine(JumpWithDelay());
        }
        else

        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        //  bool isWalking = beaverMove != Vector2.zero;
        animator.SetBool("IsJump", true);
        animator.SetBool("IsGo", false);
        animator.SetBool("IsGoBack", false);

        // audioSource.Stop();
        // audioSource.PlayOneShot(jumpSound);

        // Зупиняємо звук кроків перед стрибком
        // SoundManager.Instance.StopWalkSound();


        // Відтворюємо звук стрибка
        // SoundManager.Instance.PlayOneShot(SoundManager.Instance.jumpSound);

        Debug.Log("Force is: " + rb.linearVelocity);

    }

    private IEnumerator JumpWithDelay()
    {

        isJumping = true;
        // Чекаємо 7 кадрів
        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    public void offJump()
    {
        animator.SetBool("IsJump", false);
        isJumping = false;
        animator.SetBool("IsGo", isWalking);
        animator.SetBool("IsGoBack", isWalking);
    }

    public void stickFly(InputAction.CallbackContext context)
    {


        // StartCoroutine(ThrowAnimation());
        if (GlobalResources.Firewood > 0)
        {
            //  audioSource.PlayOneShot(hitSound);
            if (Time.time - lastThrowTime < throwCooldown)
                return; // Якщо ще не минуло 0.5 секунди, виходимо

            lastThrowTime = Time.time; // Оновлюємо час останнього кидка
            // animator.SetBool("IsThrow", true);

            // Створюємо stick у точці кидка
            GameObject stick = Instantiate(stickPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = stick.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = transform.forward * -throwForce + Vector3.up * upForce;

                GlobalResources.Firewood -= 1;

                firewoodText.text = "" + GlobalResources.Firewood;

                allSum -= 1;

                Debug.Log(" Sticks rest  " + addSum);
                Debug.Log(" Sticks rest  " + addSum + " sticks.Length  " + sticks.Length);




                int activeCount = Mathf.CeilToInt(allSum / perOne);

                for (int i = sticks.Length - 1; i >= 0; i--)
                {
                    if (sticks[i] != null)
                    {
                        sticks[i].SetActive(i < activeCount);
                    }
                }

                calcSum = allSum;
                //  SoundManager.Instance.PlayOneShot(SoundManager.Instance.flyStickSound);
            }
        }
        else
        {
            firewoodText.text = "X";
            //  audioSource.PlayOneShot(hitFailSound);
            //            SoundManager.Instance.PlayOneShot(SoundManager.Instance.emptyStickSound);
        }
    }

    public IEnumerator ThrowAnimation()
    {
        animator.SetBool("IsThrow", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsThrow", false);
    }

    public void DoorAnimOn()
    {
        BaseDoor.SetActive(false);
        AnimDoor.SetActive(true);
    }

    public void DoorAnimOff()
    {
        BaseDoor.SetActive(true);
        AnimDoor.SetActive(false);
    }
}
