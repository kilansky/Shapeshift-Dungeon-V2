﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunTimer : SingletonPattern<RunTimer>
{
    public TextMeshProUGUI timerText;
    public string TimerTextValue { get; set; }

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

        if(timeSpan.Hours > 0)
        {
            TimerTextValue = timeSpan.Hours.ToString("0") + ":" +
                timeSpan.Minutes.ToString("00") + ":" +
                timeSpan.Seconds.ToString("00");
        }
        else
        {
            TimerTextValue = timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00");
        }

        timerText.text = TimerTextValue;
    }

    public string FinalTime()
    {
        var timeSpan = System.TimeSpan.FromSeconds(runTimer);
        string time;

        time = timeSpan.Hours.ToString("0") + ":" + timeSpan.Minutes.ToString("00") + ":" + timeSpan.Seconds.ToString("00") + timeSpan.Milliseconds.ToString("00");
        return string.Format(@"{0:mm\:ss\:ff}", time);
    }
}