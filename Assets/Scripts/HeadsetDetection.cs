using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private FloorPrefabPlacer floorPrefabPlacer;
    

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
        // Start a coroutine that waits until a plant is placed.
        StartCoroutine(WaitForPlantThenStartTimer());
    }

    private IEnumerator WaitForPlantThenStartTimer()
    {
        uiCanvas.SetActive(false);
        infoCanvas.SetActive(true);
        while (!floorPrefabPlacer.GetCurrentPlantStatus())
        {
            Debug.Log("Plant status is false. Waiting for plant to be placed...");
            textTMP.text = "Please place a plant before starting a timer session.";
            subTextTMP.text = "";
            yield return null;
        }
        Debug.Log("Plant status is true. Proceeding with timer session.");
        
        // Once a plant is placed, start the timer session.
        isTimerSessionActive = true;
        Debug.Log("Timer session is now active.");
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
        Debug.Log("HMDUnmounted event triggered.");
        Debug.Log($"isTimerSessionActive: {isTimerSessionActive}");
        Debug.Log($"remainingTime: {remainingTime}");
        Debug.Log($"isTimerRunning: {isTimerRunning}");
        Debug.Log($"Plant status: {floorPrefabPlacer.GetCurrentPlantStatus()}");
        if (isTimerSessionActive && remainingTime > 0 && !isTimerRunning)
        {
            timerStartTime = DateTime.UtcNow;
            timerEndTime = timerStartTime.Value.AddSeconds(remainingTime);
            isTimerRunning = true;
            subTextTMP.text = "";
            Debug.Log("HMDUnmounted: Timer started.");

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
        PlantGrowing plantGrowing = GameObject.FindObjectOfType<PlantGrowing>();    
        if (plantGrowing != null)
        {
            plantGrowing.AdvanceToNextStage();
            int currentProgress = PlayerPrefs.GetInt("PlantProgress", 0);
            currentProgress++; 
            PlayerPrefs.SetInt("PlantProgress", currentProgress);
            PlayerPrefs.Save();
        
           
            if (currentProgress >= plantGrowing.GetTotalStages())
            {
                Destroy(plantGrowing);
                floorPrefabPlacer.SetCurrentPlantGrowing(false);
            }
        }
    
        // Update UI texts as needed.
        textTMP.text = "";
        subTextTMP.text = "Success! Your plant is growing.";
    
        // End the current timer session.
        isTimerSessionActive = false;
        
    }
}