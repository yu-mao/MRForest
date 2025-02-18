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
        Vector3 forward = attachmentPoint.transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 newPosition = attachmentPoint.transform.position + forward * 0.5f;
        menu.transform.position = newPosition;
        infoCanvas.transform.position = newPosition;
        
        Quaternion yRotation = Quaternion.Euler(
            0, 
            attachmentPoint.transform.rotation.eulerAngles.y, 
            0
        );
        
        menu.transform.rotation = yRotation;
        infoCanvas.transform.rotation = yRotation;
        
        if (OVRInput.GetDown(OVRInput.RawButton.Start))
        {

            menu.SetActive(!menu.activeSelf);
        }
    }
}
