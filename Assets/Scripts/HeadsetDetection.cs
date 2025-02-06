using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class HeadsetDetection : MonoBehaviour
{
    public GameObject infoCanvas;
    public GameObject uiCanvas;
    public TMP_Text timerText;
    public TMP_Text textTMP;    
    public TMP_Text subTextTMP; 
    private float timerDuration;
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
                timerText.text = TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss");
                textTMP.text = "Timer completed.";
            }
            else
            {
                timerText.text = TimeSpan.FromSeconds(remainingTime).ToString(@"mm\:ss");
                
            }
        }
    }
    
    public void StartTimerSession()
    {
        uiCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        isTimerSessionActive = true;
        timerCompleted = false;
        remainingTime = timerDuration;
        timerStartTime = null;
        timerEndTime = null;
        isTimerRunning = false;
        textTMP.text = "Your plant needs fresh air! Please remove your headset to help it grow.";
        subTextTMP.text = "Remove headset to begin countdown.";
    }

    public void SetTimerDuration(float duration)
    {
        timerDuration = duration;
    }

    // Called when the headset is removed.
    private void HandleHMDUnmounted()
    {
        if (isTimerSessionActive && remainingTime > 0 && !isTimerRunning)
        {
            timerStartTime = DateTime.UtcNow;
            timerEndTime = timerStartTime.Value.AddSeconds(remainingTime);
            isTimerRunning = true;
            subTextTMP.text = "";
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
                textTMP.text = "Your plant needs fresh air! Please remove your headset to help it grow.";
                subTextTMP.text = "Remove headset to resume countdown.";
            }
            else if (timerCompleted)
            {
                OnGrowPlantSuccess();
            }
        }
    }
    
    private void OnGrowPlantSuccess()
    {
        textTMP.text = ""; 
        subTextTMP.text = "Success! Your plant is growing.";
        isTimerSessionActive = false;
        
    }
}