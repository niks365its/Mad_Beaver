using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class Control : MonoBehaviour

{

    private Input input;

    private bool isMoveLeft = false;
    private bool isMoveRight = false;
    private bool isJumping;
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
    private bool IsStickFly;
    private int groundContacts = 0;

    // Start is called before the first frame update
    void Awake()

    {
        input = new Input();

        input.player.Left.performed += moveLeft;
        input.player.Left.canceled += stopLeft;
        input.player.Right.performed += moveRight;
        input.player.Right.canceled += stopRight;
        input.player.Jump.performed += onJump;
        input.player.Throw.performed += stickFly;
        input.player.Throw.canceled += stickNoFly;

    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();
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



    }

    // private void FixedUpdate()
    // {
    //     if (!isGrounded)
    //     {
    //         animator.SetBool("IsJump", true);
    //     }
    //     else
    //     {
    //         animator.SetBool("IsJump", false);
    //     }
    // }

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
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            // animator.SetBool("IsJump", true);
        }
    }

    // public void OnStopJumping()
    // {

    //     // animator.SetBool("IsJump", false);
    //     isJumping = false;


    // }

    public void stickFly(InputAction.CallbackContext context)
    {
        animator.SetBool("IsThrow", true);
        // Створюємо камінчик у точці кидка
        GameObject stick = Instantiate(stickPrefab, throwPoint.position, throwPoint.rotation);
        Rigidbody2D rb = stick.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Визначаємо напрямок кидка залежно від напряму персонажа
            float direction = transform.localScale.x > 0 ? 1f : -1f; // Напрямок: 1 - вправо, -1 - вліво
            rb.velocity = new Vector2(throwForce * direction, 0); // Кидок по горизонталі
        }
        //Destroy(stick, 5f); // Знищення через 5 секунд
        // animator.SetBool("IsThrow", false);
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



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts++;
            if (groundContacts == 1) // Якщо це перший контакт із землею, то вважаємо персонажа приземленим
            {
                isGrounded = true;
                animator.SetBool("IsJump", false);
                Debug.Log("Grounded");
                Debug.Log("Is Not Jumping");
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
                Debug.Log("Not Grounded");
                Debug.Log("Is Jumping");
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
