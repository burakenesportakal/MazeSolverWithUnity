using UnityEngine;
using TMPro;
using System.Globalization;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    public TextMeshProUGUI timerText;
    private float startTime;
    private float finalTime = 0f;
    private bool isRunning = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isRunning)
        {
            float elapsed = Time.time - startTime;
            timerText.text = "Time: " + elapsed.ToString("F2", CultureInfo.InvariantCulture) + "s";
        }
        else
        {
            // Zaman durunca sabit zamanı göster
            timerText.text = "Time: " + finalTime.ToString("F2", CultureInfo.InvariantCulture) + "s";
        }
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        if (isRunning)
        {
            finalTime = Time.time - startTime;
            isRunning = false;
        }
    }

    public float GetElapsedTime()
    {
        return finalTime;
    }
}
