using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlantGrowing : MonoBehaviour
{
    public int currentStageIndex = 0;
    
    [SerializeField] private List<GameObject> _plantsInEachGrowingStage;
    [SerializeField] private List<GameObject> _vfxInEachGrowingStage;

    [SerializeField] private float _maxInactiveDuration = 2f; // the longest time a user is inactive before the plant withers
    
    private Animator _animator;

    public void Initialize()
    {
        // hide all plants for later object pooling
        foreach (var plant in _plantsInEachGrowingStage)
        {
            plant.SetActive(false);
        }
        currentStageIndex = 0;
    }

    public void AdvanceToNextStage()
    {
        if (currentStageIndex >= _plantsInEachGrowingStage.Count) return;
        StartCoroutine(AsyncAdvance());
    }

    private IEnumerator AsyncAdvance()
    {
        
        _animator.SetTrigger("appear");
        
        if (currentStageIndex > 0)
        {
            _plantsInEachGrowingStage[currentStageIndex - 1].SetActive(false);
            
        }
        else
        {
            // fix some animation glitch
            yield return new WaitForSeconds(0.5f);
        }

        _plantsInEachGrowingStage[currentStageIndex].SetActive(true);
        currentStageIndex++;

        yield break;
    }

    public void Wither()
    {
        _animator.SetTrigger("wither");
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
}
