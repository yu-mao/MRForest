using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCollectable : MonoBehaviour
{
    [SerializeField] private AudioSource collectSound;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") || other.CompareTag("Sun"))
        {
            other.gameObject.SetActive(false);
            collectSound.Play();
        }
    }
}
