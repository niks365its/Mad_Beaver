using UnityEngine;
using UnityEngine.UI;

public class AirBar : MonoBehaviour
{
    public Slider airSlider; // Посилання на слайдер
    public HealthBar healthBeaver;

    public float timer = 30f;
    public float maxAir = 100f; // Максимальна кількість повітря
    private float currentAir;
    private float decreaseRate;
    public float increaseRate = 50f;
    private bool isAir = false;
    private Rigidbody2D rb;
    private float defaultGravity;
    public float gravity = 1f;


    void Start()
    {
        currentAir = maxAir;
        airSlider.maxValue = maxAir;
        airSlider.value = currentAir;

        decreaseRate = maxAir / timer; // 20 секунд до нуля

        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        if (currentAir > 0f && !isAir)
        {
            currentAir -= decreaseRate * Time.deltaTime;
            currentAir = Mathf.Max(currentAir, 0f);
            airSlider.value = currentAir;

            if (currentAir == 0f)
            {
                OnDead();
            }
        }

        if (currentAir < maxAir && isAir)
        {
            currentAir += increaseRate * Time.deltaTime;
            currentAir = Mathf.Min(currentAir, maxAir); // забезпечує плавне заповнення
            airSlider.value = currentAir;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("AirBubble"))
        {
            //currentAir = maxAir;
            //airSlider.value = currentAir;
            isAir = true;
            SoundManager.Instance.PlaySound(SoundManager.Instance.airSound);
            rb.gravityScale = gravity;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("AirBubble"))
        {

            isAir = false;


            SoundManager.Instance.StopSound();
            rb.gravityScale = defaultGravity;
        }
    }



    private void OnDead()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.sharpSound);
        healthBeaver.ZeroHealth();
    }
}
