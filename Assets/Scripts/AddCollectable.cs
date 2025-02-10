using System.Collections;
using UnityEngine;

public class AddCollectable : MonoBehaviour
{
    [SerializeField] private AudioSource collectSound;
    [SerializeField] private GameObject sunParticlesEffect;
    [SerializeField] private GameObject waterParticlesEffect;
    private float particleDuration = 5f; // Duration before deactivating

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            other.gameObject.SetActive(false);
            collectSound.Play();
            ActivateParticles(waterParticlesEffect);
        }
        else if (other.CompareTag("Sun"))
        {
            other.gameObject.SetActive(false);
            collectSound.Play();
            ActivateParticles(sunParticlesEffect);
        }
    }

    private void ActivateParticles(GameObject particleEffect)
    {
        if (particleEffect != null)
        {
            particleEffect.SetActive(true);
            StartCoroutine(DeactivateAfterDelay(particleEffect, particleDuration));
        }
    }

    private IEnumerator DeactivateAfterDelay(GameObject particleEffect, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (particleEffect != null)
        {
            particleEffect.SetActive(false);
        }
    }
}