using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BreakTimeManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    private const string defaultTimePref = "DefaultTime";
    private float defaultTime;
    private HeadsetDetection headsetDetection;
    private void Awake()
    {
        defaultTime = PlayerPrefs.GetFloat(defaultTimePref, 10);
        timerText.text = TimeSpan.FromSeconds(defaultTime).ToString(@"mm\:ss");
        headsetDetection = GetComponent<HeadsetDetection>();
        headsetDetection.SetTimerDuration(defaultTime);

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseTime()
    {
        defaultTime += 10;
        headsetDetection.SetTimerDuration(defaultTime);;
        PlayerPrefs.SetFloat(defaultTimePref, defaultTime);
        PlayerPrefs.Save();
        timerText.text = TimeSpan.FromSeconds(defaultTime).ToString(@"mm\:ss");
    }

    public void DecreaseTime()
    {
        defaultTime -= 10;
        headsetDetection.SetTimerDuration(defaultTime);;
        PlayerPrefs.SetFloat(defaultTimePref, defaultTime);
        PlayerPrefs.Save();
        timerText.text = TimeSpan.FromSeconds(defaultTime).ToString(@"mm\:ss");
    }
}