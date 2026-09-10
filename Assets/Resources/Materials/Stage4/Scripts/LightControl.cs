using UnityEngine;
using System.Collections;

public class LightControl : MonoBehaviour
{
    public static LightControl Instance;

    [Header("Лампи")]
    public GameObject rearGearLight;
    public GameObject leftLights;
    public GameObject rightLights;
    public GameObject frontLight;
    public GameObject brakeLight;

    private Coroutine leftBlinkCoroutine;
    private Coroutine rightBlinkCoroutine;
    private readonly WaitForSeconds blinkInterval = new WaitForSeconds(0.5f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // -------------------------
    // Передні фари
    // -------------------------

    public void SetFrontLight(bool state)
    {
        if (frontLight != null)
            frontLight.SetActive(state);
    }

    // -------------------------
    // Стоп-сигнали
    // -------------------------

    public void SetBrakeLight(bool state)
    {
        if (brakeLight != null)
            brakeLight.SetActive(state);
    }

    // -------------------------
    // Задній хід
    // -------------------------

    public void SetRearGearLight(bool state)
    {
        if (rearGearLight != null)
            rearGearLight.SetActive(state);
    }

    // -------------------------
    // Лівий поворотник
    // -------------------------

    // public void SetLeftLights(bool state)
    // {
    //     if (leftLights != null)
    //         leftLights.SetActive(state);
    // }

    public void StartLeftBlink()
    {
        if ((leftLights != null) && (leftBlinkCoroutine == null))
            leftBlinkCoroutine = StartCoroutine(BlinkLamp(leftLights));
    }

    public void StopLeftBlink()
    {
        if (leftBlinkCoroutine != null)
        {
            StopCoroutine(leftBlinkCoroutine);
            leftBlinkCoroutine = null;
        }
        leftLights.SetActive(false);
    }

    // -------------------------
    // Правий поворотник
    // -------------------------

    // public void SetRightLights(bool state)
    // {
    //     if (rightLights != null)
    //         rightLights.SetActive(state);
    // }

    public void StartRightBlink()
    {
        if ((rightLights != null) && (rightBlinkCoroutine == null))
            rightBlinkCoroutine = StartCoroutine(BlinkLamp(rightLights));
    }

    public void StopRightBlink()
    {
        if (rightBlinkCoroutine != null)
        {
            StopCoroutine(rightBlinkCoroutine);
            rightBlinkCoroutine = null;
        }
        rightLights.SetActive(false);
    }

    // -------------------------
    // Блимання
    // -------------------------

    private IEnumerator BlinkLamp(GameObject lamp)
    {
        while (true)
        {
            lamp.SetActive(true);
            yield return blinkInterval;

            lamp.SetActive(false);
            yield return blinkInterval;
        }
    }
}