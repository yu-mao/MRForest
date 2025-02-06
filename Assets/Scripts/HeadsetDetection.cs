using System;
using UnityEngine;
using TMPro;

public class HeadsetDetection : MonoBehaviour
{
    public TMP_Text timerTMP;    
    public TMP_Text messageTMP; 
    public float timerDuration = 120f;
    private float remainingTime;
    private bool isTimerSessionActive = false;
    private bool timerCompleted = false;
    private DateTime? timerStartTime = null;
    private DateTime? timerEndTime = null;
    private bool isTimerRunning = false;

    void Start()
    {
        OVRManager.HMDMounted += HandleHMDMounted;
        OVRManager.HMDUnmounted += HandleHMDUnmounted;
        remainingTime = timerDuration;
    }

    void OnDestroy()
    {
        OVRManager.HMDMounted -= HandleHMDMounted;
        OVRManager.HMDUnmounted -= HandleHMDUnmounted;
    }

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            isTimerSessionActive = true;
            timerCompleted = false;
            remainingTime = timerDuration;
            timerStartTime = null;
            timerEndTime = null;
            isTimerRunning = false;
            timerTMP.text = "Timer not started. Remove headset to begin countdown.";
            messageTMP.text = "";
        }

        if (isTimerRunning && timerEndTime.HasValue)
        {
            TimeSpan timeLeft = timerEndTime.Value - DateTime.UtcNow;
            remainingTime = (float)timeLeft.TotalSeconds;

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                timerCompleted = true;
                isTimerRunning = false;
                timerStartTime = null;
                timerEndTime = null;
                timerTMP.text = "Time remaining: 0.0 s";
                messageTMP.text = "Timer completed. ";
            }
            else
            {
                timerTMP.text = "Time remaining: " + remainingTime.ToString("F1") + " s";
            }
        }
    }

    // Called when the headset is removed.
    private void HandleHMDUnmounted()
    {
        if (isTimerSessionActive && remainingTime > 0 && !isTimerRunning)
        {
            timerStartTime = DateTime.UtcNow;
            timerEndTime = timerStartTime.Value.AddSeconds(remainingTime);
            isTimerRunning = true;
            messageTMP.text = "";
        }
    }

    // Called when the headset is put on.
    private void HandleHMDMounted()
    {
        if (isTimerSessionActive)
        {
            if (isTimerRunning)
            {
                TimeSpan timeLeft = timerEndTime.Value - DateTime.UtcNow;
                remainingTime = (float)timeLeft.TotalSeconds;
                isTimerRunning = false;
                timerStartTime = null;
                timerEndTime = null;
                timerTMP.text = "Time remaining: " + remainingTime.ToString("F1") + " s";
                messageTMP.text = "Warning: Timer paused. Please keep the headset off.";
            }
            else if (timerCompleted)
            {
                timerTMP.text = "Time remaining: 0.0 s";
                messageTMP.text = "Success";
                isTimerSessionActive = false; // End the current session.
            }
        }
    }
}
