using UnityEngine;
using UnityEngine.SceneManagement; // para cargar escenas o mostrar Game Over
using TMPro;
using System.Collections;
using UnityEngine.XR.ARFoundation;

public class Timer : MonoBehaviour
{
    public float timeRemaining = 600f;  // tiempo inicial en segundos
    public bool timerIsRunning = false;
    public TextMeshProUGUI timerText;

    public GameObject RetryButton;
    public GameObject InventoryButton;

    public GameObject arSession; // Asigna esto desde el Inspector

    public static Timer Instance;

    public void StartTimer()
    {
        timerIsRunning = true;  // empieza el temporizador
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;


                NotificationManager.Instance.finalMessage("GAME OVER");
                // Llama al método de Game Over
                GameOver();
            }
        }
    }

    public void GameOver()
    {
        Debug.Log("Terminado");

        StartCoroutine(PauseAfterDelay());

        RetryButton.SetActive(true);
        InventoryButton.SetActive(false);

        foreach (var collider in FindObjectsOfType<Collider>())
        {
            collider.enabled = false;
        }


    }

    void UpdateTimerDisplay(float timeToDisplay)
    {
        timeToDisplay += 1; // redondeo visual

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator PauseAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 0f;
    }

}
