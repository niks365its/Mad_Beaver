using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control : MonoBehaviour

{

    private Input input;

    private bool isMoveLeft = false;
    private bool isMoveRight = false;
    //private bool isJumping;
    private bool isGrounded = false;

    public Animator animator;

    public GameObject gameOverMenu;

    public GameObject Player;
    private bool isGameOver = false;

    private Rigidbody2D rb;
    public float jumpForce = 5f;
    public float pauseTime = 5f;

    public GameObject stickPrefab; // Префаб камінчика
    public Transform throwPoint;  // Точка, з якої кидатиметься камінчик
    public float throwForce = 10f; // Сила кидка
                                   //  private bool IsStickFly;
    private int groundContacts = 0;
    public Text firewoodText;

    private float previousY;
    private float addForce = 0f;

    private float lastThrowTime = 0f;
    private float throwCooldown = 0.3f; // Час між кидками

    // Start is called before the first frame update
    void Awake()

    {
        input = new Input();

        input.player.Left.performed += moveLeft;
        input.player.Left.canceled += stopLeft;
        input.player.Right.performed += moveRight;
        input.player.Right.canceled += stopRight;
        input.player.Jump.performed += onJump;
        input.player.AngleJump.performed += onAngleJump;
        input.player.Throw.performed += stickFly;
        input.player.Throw.canceled += stickNoFly;


    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        previousY = transform.position.y;
    }


    // Update is called once per frame
    private void Update()
    {
        if (isMoveLeft)
        {
            transform.position += Vector3.left * 5 * Time.deltaTime;
            animator.SetBool("IsGo", true);
            //  animator.SetBool("IsJump", false);
        }


        if (isMoveRight)
        {
            transform.position += Vector3.right * 5 * Time.deltaTime;
            animator.SetBool("IsGo", true);
            //animator.SetBool("IsJump", false);
        }

        // Зчитування поточної позиції по осі Y
        float currentY = transform.position.y;

        // Перевірка напрямку переміщення
        if (currentY > previousY)
        {
            addForce = 2f;
        }
        else if (currentY < previousY)
        {
            addForce = -2f;
        }

        else
        {
            addForce = 0f;
        }

        // Оновлення попередньої позиції
        previousY = currentY;

    }


    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void moveLeft(InputAction.CallbackContext context)
    {
        isMoveLeft = true;
        Vector3 scale = transform.localScale;
        scale.x = -0.25f; // Змінюємо знак по осі X
        transform.localScale = scale;

        //  animator.SetBool("IsJump", false);
    }
    private void stopLeft(InputAction.CallbackContext context)
    {
        isMoveLeft = false;
        animator.SetBool("IsGo", false);
    }

    private void moveRight(InputAction.CallbackContext context)
    {
        isMoveRight = true;
        Vector3 scale = transform.localScale;
        scale.x = 0.25f; // Змінюємо знак по осі X
        transform.localScale = scale;
        // animator.SetBool("IsJump", false);
    }

    private void stopRight(InputAction.CallbackContext context)
    {
        isMoveRight = false;
        animator.SetBool("IsGo", false);
    }


    public void onJump(InputAction.CallbackContext context)
    {
        if (isGrounded) // Only jump if grounded
        {

            rb.velocity = new Vector2(rb.velocity.x, jumpForce + addForce);

            Debug.Log("Force is: " + rb.velocity);
        }
    }

    public void onAngleJump(InputAction.CallbackContext context)
    {
        if (isGrounded) // Only jump if grounded
        {
            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.velocity = new Vector2(jumpForce * 0.5f * direction, jumpForce + addForce);

            Debug.Log("Force is: " + rb.velocity);
        }
    }



    public void stickFly(InputAction.CallbackContext context)
    {
        animator.SetBool("IsThrow", true);
        if (GlobalResources.Firewood > 0)
        {
            GlobalResources.Firewood -= 1;
            firewoodText.text = "" + GlobalResources.Firewood;
            if (Time.time - lastThrowTime < throwCooldown)
                return; // Якщо ще не минуло 0.5 секунди, виходимо

            lastThrowTime = Time.time; // Оновлюємо час останнього кидка
            // animator.SetBool("IsThrow", true);

            // Створюємо stick у точці кидка
            GameObject stick = Instantiate(stickPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody2D rb = stick.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Визначаємо напрямок кидка залежно від напряму персонажа
                float direction = transform.localScale.x > 0 ? 1f : -1f;
                rb.velocity = new Vector2(throwForce * direction, 0);
            }
        }
        else
            firewoodText.text = "X";
    }

    public void stickNoFly(InputAction.CallbackContext context)
    {
        animator.SetBool("IsThrow", false);
    }

    public void OnPause()
    {
        //animator.SetBool("IsThrow", false);
        animator.SetBool("pause", true);
    }

    public void EndPause()
    {
        animator.SetBool("pause", false);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Sharp"))
        {
            Debug.Log("Game Over Trigger Set");
            animator.SetBool("IsJump", false);
            animator.SetTrigger("GameOverTrigger");
            Debug.Log("Is Sharp collision");
            Debug.Log("Game Over Trigger Set");


        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts++;
            if (groundContacts == 1) // Якщо це перший контакт із землею, то вважаємо персонажа приземленим
            {
                isGrounded = true;
                animator.SetBool("IsJump", false);

            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {


        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts--;
            if (groundContacts <= 0) // Якщо всі контакти зникли, то персонаж у повітрі
            {
                isGrounded = false;
                animator.SetBool("IsJump", true);

            }
        }


    }

    public void TriggerGameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            animator.SetTrigger("GameOverTrigger");

            // Додатково: зупинити рух або інші дії персонажа
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            // GetComponent<PlayerMovement>().enabled = false; // Якщо є скрипт руху

            // Скидання життів до 3
            HealthBar.life = 3; // Оновлення статичної змінної
            StartCoroutine(GameOverMenu());
        }
    }



    public IEnumerator GameOverMenu()
    {
        yield return new WaitForSeconds(3f);

        gameOverMenu.SetActive(true);
        Player.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        // Закриваємо гру (працює тільки у збірці)
        Application.Quit();
        // Для редактора:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
