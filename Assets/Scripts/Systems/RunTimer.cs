using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunTimer : SingletonPattern<RunTimer>
{
    public TextMeshProUGUI timerText;
    public string TimerTextValue { get; set; }
    public string EndTimeValue { get; set; }

    private float runTimer = 0;
    public bool IncreaseTimer { get; set; }

    private void Start()
    {
        IncreaseTimer = false;
    }

    void Update()
    {
        if(IncreaseTimer)
        {
            runTimer += Time.deltaTime;
            runTimer = Mathf.Clamp(runTimer, 0, 35999); //Clamp time to 9 hrs, 59 min, 59 sec
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        var timeSpan = System.TimeSpan.FromSeconds(runTimer);

        //timerText.text = System.TimeSpan.FromSeconds(runTimer);

        if (timeSpan.Hours > 0)
        {
            TimerTextValue = timeSpan.ToString("h\\:mm\\:ss");
            EndTimeValue = timeSpan.ToString("h\\:mm\\:ss\\.ff");
        }
        else
        {
            TimerTextValue = timeSpan.ToString("mm\\:ss");
            EndTimeValue = timeSpan.ToString("mm\\:ss\\.ff");
        }

        timerText.text = TimerTextValue;
    }
}