using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class Timer : MonoBehaviour
{
    public static Timer Instance;

    public TMP_Text timerText; // UI text referansı
    private float startTime;
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
    }

    public void StartTimer()
    {
        startTime = Time.time;
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
