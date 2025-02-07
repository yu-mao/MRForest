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
        
        if (currentStageIndex > 0) _plantsInEachGrowingStage[currentStageIndex - 1].SetActive(false);
        _plantsInEachGrowingStage[currentStageIndex].SetActive(true);
        _animator.SetTrigger("appear");
        currentStageIndex++;
    }

    public void Wither()
    {
        StartCoroutine(AsyncWither());
    }

    private IEnumerator AsyncWither()
    {
        yield return _plantsInEachGrowingStage[currentStageIndex].transform.DOShakePosition(.2f, 0.5f, 20)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
        yield return _plantsInEachGrowingStage[currentStageIndex].transform.DOScale(0.7f, 0.5f)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
}
