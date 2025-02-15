using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utils : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject attachmentPoint;
    [SerializeField] private GameObject infoCanvas;
    
    [SerializeField] public bool menuToggleEnabled = true;

    public void ExitApplication()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {
            menu.transform.position = attachmentPoint.transform.position;
            infoCanvas.transform.position = attachmentPoint.transform.position;
            menu.transform.rotation = attachmentPoint.transform.rotation;
            infoCanvas.transform.rotation = attachmentPoint.transform.rotation;
            menu.SetActive(!menu.activeSelf);
        }
        

    }
}
