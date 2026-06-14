using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Control3 : MonoBehaviour

{
    private Input input;

    private bool isMoveLeft = false;
    private bool isMoveRight = false;
    private bool isForward = false;
    private bool isBackward = false;
    private bool isGrounded = false;
    public bool isWater = false;

    public Animator animator;

    public GameObject gameOverMenu;

    public GameObject Player;
    public GameObject screenController;
    private bool isTouch = false;

    private bool isGameOver = false;

    private Rigidbody2D rb;
    public float jumpForce = 5f;
    public float pauseTime = 5f;

    public float minY = -3f;
    public float maxY = 3f;

    public float gravity = 1f;
    private float defaultGravity;

    public GameObject stickPrefab; // Префаб камінчика
    public Transform throwPoint;  // Точка, з якої кидатиметься камінчик
    public float throwForce = 10f; // Сила кидка
    private int groundContacts = 0;
    public Text firewoodText;

    private float previousY;
    private float addForce = 0f;

    public float forwardSpeed = 3f;

    private float lastThrowTime = 0f;
    private float throwCooldown = 0.3f; // Час між кидками

    private Vector2 previousPosition;
    public ParticleSystem bubbleSystem; // Посилання на систему частинок
    private ParticleSystem.VelocityOverLifetimeModule velocityModule;

    void Awake()
    {
        input = new Input();

        input.player.Left.performed += onBackward;
        input.player.Left.canceled += onStopBackward;
        input.player.Right.performed += onForward;
        input.player.Right.canceled += onStopForward;
        input.player.Jump.performed += moveLeft;
        input.player.Jump.canceled += stopLeft;
        input.player.Down.performed += moveRight;
        input.player.Down.canceled += stopRight;
        input.player.AngleJump.performed += onAngleJump;
        input.player.Throw.performed += stickFly;
    }

    private void Start()
    {
        animator.SetBool("IsJump3", false);
        SoundManager.Instance.StopEffectsSound();
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = 0;

        animator = GetComponent<Animator>();

        previousY = transform.position.y;

        GlobalResources.Firewood = 0;

        SoundManager.Instance.PlayBackgroundMusic(SoundManager.Instance.backgroundSound);

        // Отримуємо доступ до Velocity over Lifetime модуля
        velocityModule = bubbleSystem.velocityOverLifetime;
        previousPosition = transform.position;

        Debug.Log("touchScreen is: (for start)" + TouchControlsManager.Instance.IsTouch);
    }

    private void Update()
    {
        float currentSpeed = forwardSpeed;

        if (isMoveLeft)
        {
            transform.position += Vector3.up * 5 * Time.deltaTime;
        }

        if (isMoveRight)
        {
            transform.position += Vector3.down * 5 * Time.deltaTime;
        }

        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;

        if (isForward)
        {
            currentSpeed = forwardSpeed * 1.5f;
        }

        if (isBackward)
        {
            currentSpeed = forwardSpeed * 0.5f;

            Debug.Log("BackSpeed is" + currentSpeed);
        }

        transform.position += Vector3.right * currentSpeed * Time.deltaTime;
    }

    private void OnEnable()
    {
        input.Enable();

        Debug.Log("touchScreen is: (for enable)" + TouchControlsManager.Instance.IsTouch);

        if (TouchControlsManager.Instance != null && TouchControlsManager.Instance.IsTouch)
        {
            screenController.SetActive(true);
            Debug.Log("touchScreen is (for true): " + TouchControlsManager.Instance.IsTouch);
        }
        else
        {
            screenController.SetActive(false);
            Debug.Log("touchScreen is: (for false)" + TouchControlsManager.Instance.IsTouch);
        }
    }

    private void OnDisable()
    {
        input.Disable();
        if (screenController)
        {
            screenController.SetActive(false);
        }
    }

    private void moveLeft(InputAction.CallbackContext context)
    {
        isMoveLeft = true;
        Vector3 scale = transform.localScale;
        scale.y = -Mathf.Abs(scale.y); // Змінюємо знак по осі Y
        transform.localScale = scale;
    }

    private void stopLeft(InputAction.CallbackContext context)
    {
        isMoveLeft = false;
        animator.SetBool("IsGo", false);
        SoundManager.Instance.StopWalkSound();
    }

    private void moveRight(InputAction.CallbackContext context)
    {
        isMoveRight = true;
        Vector3 scale = transform.localScale;
        scale.y = Mathf.Abs(scale.y); // Змінюємо знак по осі Y
        transform.localScale = scale;
    }

    private void stopRight(InputAction.CallbackContext context)
    {
        isMoveRight = false;
        animator.SetBool("IsGo", false);
        SoundManager.Instance.StopWalkSound();
    }

    public void onForward(InputAction.CallbackContext context)
    {
        isForward = true;
    }

    public void onStopForward(InputAction.CallbackContext context)
    {
        isForward = false;
        animator.SetBool("IsSwim", true);
    }

    public void onBackward(InputAction.CallbackContext context)
    {
        isBackward = true;
    }

    public void onStopBackward(InputAction.CallbackContext context)
    {
        isBackward = false;
        animator.SetBool("IsSwim", true);
    }

    public void onAngleJump(InputAction.CallbackContext context)
    {
        animator.SetBool("IsJump3", true);
        StartCoroutine(DisableColliderCoroutine());

        Debug.Log("IsJump is: " + animator.GetBool("IsJump"));

        // Зупиняємо звук кроків перед стрибком
        SoundManager.Instance.StopWalkSound();

        // Відтворюємо звук стрибка
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.jumpSound);

        Debug.Log("Force is: " + rb.linearVelocity);
    }

    private IEnumerator DisableColliderCoroutine()
    {

        gameObject.layer = LayerMask.NameToLayer("JumpPlayer");
        yield return new WaitForSeconds(2f);
        gameObject.layer = LayerMask.NameToLayer("Player");
        animator.SetBool("IsJump3", false);
        Debug.Log("IsJump is: " + animator.GetBool("IsJump"));

    }

    public void stickFly(InputAction.CallbackContext context)
    {
        StartCoroutine(ThrowAnimation());
        if (GlobalResources.Firewood > 0)
        {
            if (Time.time - lastThrowTime < throwCooldown)
                return; // Якщо ще не минуло 0.5 секунди, виходимо

            lastThrowTime = Time.time; // Оновлюємо час останнього кидка

            // Створюємо stick у точці кидка
            GameObject stick = Instantiate(stickPrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody2D rb = stick.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Визначаємо напрямок кидка залежно від напряму персонажа
                float direction = transform.localScale.x > 0 ? 1f : -1f;
                rb.linearVelocity = new Vector2(throwForce * direction, 0);

                if (isWater)
                {
                    // Додаємо обертання кулі під час її руху
                    rb.angularVelocity = -300f;
                }

                GlobalResources.Firewood -= 1;
                firewoodText.text = "" + GlobalResources.Firewood;

                SoundManager.Instance.PlayOneShot(SoundManager.Instance.flyStickSound);
            }
        }
        else
        {
            firewoodText.text = "X";
            SoundManager.Instance.PlayOneShot(SoundManager.Instance.emptyStickSound);
        }
    }

    public IEnumerator ThrowAnimation()
    {
        animator.SetBool("IsThrow", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsThrow", false);
    }

    public void OnPause()
    {
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
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !isGameOver)
        {
            groundContacts--;
            if (groundContacts <= 0) // Якщо всі контакти зникли, то персонаж у повітрі
            {
                isGrounded = false;
                if (isWater)
                {
                    animator.SetBool("IsSwim", true);
                }
                animator.SetBool("IsGo", false);
                SoundManager.Instance.StopWalkSound();
            }
        }
    }

    public void TriggerGameOver()
    {
        if (!isGameOver)
        {
            screenController.SetActive(false);
            isGameOver = true;
            isGrounded = true;
            animator.SetBool("IsSwim", false);

            animator.SetBool("IsDead", true);

            // Додатково: зупинити рух або інші дії персонажа
            GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Скидання життів до 3
            HealthBar.life = 3; // Оновлення статичної змінної
            StartCoroutine(GameOverMenu());
        }
    }

    public IEnumerator GameOverMenu()
    {
        screenController.SetActive(false);
        yield return new WaitForSeconds(3f);
        // Зупиняємо звуки
        SoundManager.Instance.StopEffectsSound();
        SoundManager.Instance.StopBackgroundSound();

        gameOverMenu.SetActive(true);
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.gameOverSound);
        Player.SetActive(false);
    }

    public void RestartGame()
    {
        SoundManager.Instance.StopEffectsSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void NextLevel(int level)
    {
        SoundManager.Instance.StopEffectsSound();
        SceneManager.LoadScene(level);
        FindObjectOfType<GameMenus>().LevelCompleted(level);
    }

    public void ExitGame()
    {
        // Закриваємо гру (працює тільки у збірці)
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void TouchLeft()
    {
        var fakeContext = new InputAction.CallbackContext();
        moveLeft(fakeContext);
    }

    public void TouchRight()
    {
        var fakeContext = new InputAction.CallbackContext();
        moveRight(fakeContext);
    }

    public void UnTouchLeft()
    {
        var fakeContext = new InputAction.CallbackContext();
        stopLeft(fakeContext);
    }

    public void UnTouchRight()
    {
        var fakeContext = new InputAction.CallbackContext();
        stopRight(fakeContext);
    }

    public void TouchJump()
    {
        var fakeContext = new InputAction.CallbackContext();
    }

    public void TouchThrow()
    {
        var fakeContext = new InputAction.CallbackContext();
        stickFly(fakeContext);
    }
}