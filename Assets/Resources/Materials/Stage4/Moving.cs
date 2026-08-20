using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Moving : MonoBehaviour
{
    private Input input;
    public GameObject Avto;
    public float speed = 5f;
    public float rotationSpeed = 100f;

    public float jumpForce = 30f;
    private Vector2 beaverMove;

    public Animator animator;

    private Rigidbody rb;

    private bool isJumping = false;
    private bool isWalking = false;

    void Awake()

    {
        input = new Input();

        input.player.CarMove.performed += moveBeaver;
        input.player.CarMove.canceled += moveBeaver;
        input.player.AngleJump.performed += onJump;

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
        animator.SetBool("IsGo", isWalking);
        animator.SetBool("IsJump", false);
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

        //rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

        //  bool isWalking = beaverMove != Vector2.zero;
        animator.SetBool("IsJump", true);
        animator.SetBool("IsGo", false);

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
    }
}
