using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class CurrentProgress : MonoBehaviour
{
    [SerializeField] private List<Slider> sliders;
    [SerializeField] private List<TMP_Text> texts;
    // Start is called before the first frame update
    void Start()
    {
       
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProgress(int progress, int total)
    {
        float value = (float)progress / total;
        for (int i = 0; i < sliders.Count; i++)
        {
            sliders[i].value = value;
            texts[i].text = (value * 100f).ToString("F0") + "%"; // formats the percentage with no decimals
        }
    }
}
