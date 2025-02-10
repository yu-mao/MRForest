using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

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
    [SerializeField] private GameObject successGameObject;
    [SerializeField] private GameObject failureGameObject;
    [SerializeField] private GameObject interactionGameObject;
    [SerializeField] private Image panelImage;
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
            textTMP.text = "Please place a plant before starting a timer session.";
            subTextTMP.text = "";
            yield return null;
        }
        
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
        panelImage.color = new Color32(0, 33, 21, 203);
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
                SetGameObjectActiveRecursively(uiCanvas, false);
                panelImage.GameObject().SetActive(true);
                panelImage.color = new Color32(110, 18, 0, 203);
                uiCanvas.SetActive(true);
                failureGameObject.SetActive(true);
                interactionGameObject.SetActive(true);

            }
            else if (timerCompleted)
            {
                SetGameObjectActiveRecursively(uiCanvas, false);
                panelImage.GameObject().SetActive(true);
                uiCanvas.SetActive(true);
                successGameObject.SetActive(true);
                interactionGameObject.SetActive(true);
                OnGrowPlantSuccess();
            }
        }
    }
    
    private void OnGrowPlantSuccess()
    {
        // Retrieve the active plant from FloorPrefabPlacer.
        PlantGrowing plantGrowing = floorPrefabPlacer.GetActivePlantGrowingComponent();
        if (plantGrowing != null)
        {
            OVRSpatialAnchor anchor = plantGrowing.GetComponentInParent<OVRSpatialAnchor>();

            if (anchor != null)
            {
                string progressKey = "PlantProgress_" + anchor.Uuid.ToString();
                int currentProgress = PlayerPrefs.GetInt(progressKey, 0);
                int totalStages = plantGrowing.GetTotalStages();
                Debug.Log($"[OnGrowPlantSuccess] currentProgress = {currentProgress}, TotalStages = {totalStages}");

                if (currentProgress < totalStages)
                {
                    //Give yourself a little more rest so you can get full reward 
                    //to make your plant grow strong!
                    plantGrowing.AdvanceToNextStage();
                    currentProgress++;
                    PlayerPrefs.SetInt(progressKey, currentProgress);
                    PlayerPrefs.Save();
                    Debug.Log($"[OnGrowPlantSuccess] Advanced to stage {currentProgress}");
            
                    if (currentProgress >= totalStages)
                    {
                        Debug.Log("[OnGrowPlantSuccess] Plant reached full growth. Clearing active plant flag and resetting progress.");
                        floorPrefabPlacer.SetCurrentPlantGrowing(false);
                        PlayerPrefs.DeleteKey(FloorPrefabPlacer.LastCreatedAnchorUUIDKey);
                        PlayerPrefs.DeleteKey(progressKey);  // Clean up when fully grown
                        PlayerPrefs.Save();
                    }
                }
                else
                {
                    Debug.Log("[OnGrowPlantSuccess] Plant is already fully grown. Clearing active plant flag and resetting progress.");
                    floorPrefabPlacer.SetCurrentPlantGrowing(false);
                    PlayerPrefs.DeleteKey(FloorPrefabPlacer.LastCreatedAnchorUUIDKey);
                    PlayerPrefs.DeleteKey(progressKey);  // Clean up when fully grown
                    PlayerPrefs.Save();
                }
            }
            else
            {
                Debug.LogError("[OnGrowPlantSuccess] Could not find OVRSpatialAnchor component!");
            } 
            
        }
        else
        {
            Debug.Log("[OnGrowPlantSuccess] No active PlantGrowing component found.");
        }
    
        textTMP.text = "";
        subTextTMP.text = "Success! Your plant is growing.";
        isTimerSessionActive = false;
        Debug.Log("[OnGrowPlantSuccess] Timer session ended.");
    }
    
    private void SetGameObjectActiveRecursively(GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}