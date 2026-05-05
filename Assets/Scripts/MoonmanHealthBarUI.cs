using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoonmanHealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text labelText;

    [Header("Fizz Effect")]
    [SerializeField] private float fizzDuration = 0.25f;
    [SerializeField] private float fizzSpeed = 40f;
    [SerializeField] private float minFlickerAlpha = 0.45f;
    [SerializeField] private float maxFlickerAlpha = 1f;

    private Coroutine fizzRoutine;
    private Color originalFillColor;
    private Color originalLabelColor;

    private void Awake()
    {
        if (fillImage != null)
        {
            originalFillColor = fillImage.color;
        }

        if (labelText != null)
        {
            originalLabelColor = labelText.color;
        }
    }

    public void SetHealth(float normalizedHealth)
    {
        normalizedHealth = Mathf.Clamp01(normalizedHealth);

        if (fillImage != null)
        {
            fillImage.fillAmount = normalizedHealth;
        }
    }

    public void Fizz()
    {
        if (fizzRoutine != null)
        {
            StopCoroutine(fizzRoutine);
        }

        fizzRoutine = StartCoroutine(FizzRoutine());
    }

    private IEnumerator FizzRoutine()
    {
        float timer = 0f;

        while (timer < fizzDuration)
        {
            timer += Time.deltaTime;

            float flicker = Mathf.Lerp(
                minFlickerAlpha,
                maxFlickerAlpha,
                Mathf.PingPong(Time.time * fizzSpeed, 1f)
            );

            if (fillImage != null)
            {
                Color c = originalFillColor;
                c.a = flicker;
                fillImage.color = c;
            }

            if (labelText != null)
            {
                Color c = originalLabelColor;
                c.a = flicker;
                labelText.color = c;
            }

            yield return null;
        }

        if (fillImage != null)
        {
            fillImage.color = originalFillColor;
        }

        if (labelText != null)
        {
            labelText.color = originalLabelColor;
        }

        fizzRoutine = null;
    }
}
