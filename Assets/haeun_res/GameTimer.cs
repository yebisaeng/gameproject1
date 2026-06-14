using System.Diagnostics;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public TMP_Text timerText;
    public float maxTime = 60f;
    public float currentTime;

    public Game_SET_main_end gameManager;
    private bool isGameOverTriggered = false;

    public float RemainingTime => currentTime;
    public bool IsRunning => currentTime > 0f;

    void Start()
    {
        currentTime = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0f) return;
        currentTime -= Time.deltaTime;

        if (currentTime < 0)
        {
            currentTime = 0;

            if (!isGameOverTriggered)
            {
                isGameOverTriggered = true;

                if (gameManager != null)
                {
                    gameManager.CheckGameResult();
                }
            }
        }
            

        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void AddTime(float amount)
    {
        currentTime = Mathf.Min(currentTime + amount, maxTime);
    }
}