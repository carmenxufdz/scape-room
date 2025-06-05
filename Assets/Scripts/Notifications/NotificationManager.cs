using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Solo si usas TextMeshPro

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance;

    public TextMeshProUGUI notificationText; // O usa Text si no usas TMP
    public CanvasGroup canvasGroup;

    private float fadeDuration = 0.5f;
    private float displayTime = 2f;

    private Coroutine currentRoutine;

    private Coroutine finalRoutine;

    void Awake()
    {
        Instance = this;
        canvasGroup.alpha = 0;
    }

    public void ShowMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndFade(message));
    }

    private IEnumerator ShowAndFade(string message)
    {
        notificationText.text = message;

        // Fade In
        float t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

        yield return new WaitForSeconds(displayTime);

        // Fade Out
        t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    private IEnumerator Show(string message)
    {
        notificationText.text = message;

        // Fade In
        float t = 0f;
        while (t < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;

    }

    public void finalMessage(string message)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        finalRoutine = StartCoroutine(Show(message));
    }
}
