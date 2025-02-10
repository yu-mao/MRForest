using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCollectable : MonoBehaviour
{
    [SerializeField] private AudioSource collectSound;
    private int rewardCollected;
    [SerializeField] private ParticleSystem waterCollectedParticles;
    [SerializeField] private ParticleSystem sunCollectedParticles;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") || other.CompareTag("Sun"))
        {
            if (other.CompareTag("Water"))
            {
                waterCollectedParticles.Play();
            }
            else
            {
                sunCollectedParticles.Play();
            }
            other.gameObject.SetActive(false);
            collectSound.Play();
            rewardCollected++;
        }
    }

    private void Update()
    {
        if (rewardCollected >= 10)
        {
            FindObjectOfType<HeadsetDetection>().isRewardCollected = true;
        }
    }
}
