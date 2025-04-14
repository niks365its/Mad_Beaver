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


    void Start()
    {
        currentAir = maxAir;
        airSlider.maxValue = maxAir;
        airSlider.value = currentAir;

        decreaseRate = maxAir / timer; // 20 секунд до нуля
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

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("AirBubble"))
        {

            isAir = false;


            SoundManager.Instance.StopSound();

        }
    }



    private void OnDead()
    {
        SoundManager.Instance.PlayOneShot(SoundManager.Instance.sharpSound);
        healthBeaver.ZeroHealth();
    }
}
