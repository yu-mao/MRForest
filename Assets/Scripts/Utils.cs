using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utils : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    
    public void ExitApplication()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            menu.SetActive(!menu.activeSelf);
        }
        

    }
}
