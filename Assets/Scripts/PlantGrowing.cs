using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlantGrowing : MonoBehaviour
{
    [SerializeField] private List<GameObject> _plantsInEachGrowingStage;
    [SerializeField] private List<GameObject> _vfxInEachGrowingStage;

    [SerializeField] private float _maxInactiveDuration = 2f; // the longest time a user is inactive before the plant withers

    private int _currentStageIndex = 0;

    public void Initialize()
    {
        // hide all plants for later object pooling
        foreach (var plant in _plantsInEachGrowingStage)
        {
            plant.SetActive(false);
        }
        _currentStageIndex = 0;
    }

    public void AdvanceToNextStage()
    {
        if (_currentStageIndex >= _plantsInEachGrowingStage.Count) return;
        
        if (_currentStageIndex > 0) _plantsInEachGrowingStage[_currentStageIndex - 1].SetActive(false);
        _plantsInEachGrowingStage[_currentStageIndex].SetActive(true);
        _currentStageIndex++;
    }

    public void Wither()
    {
        StartCoroutine(AsyncWither());
    }

    private IEnumerator AsyncWither()
    {
        yield return _plantsInEachGrowingStage[_currentStageIndex].transform.DOShakePosition(.2f, 0.5f, 20)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
        yield return _plantsInEachGrowingStage[_currentStageIndex].transform.DOScale(0.7f, 0.5f)
            .SetEase(Ease.InOutQuad)
            .WaitForCompletion();
    }
    
    public int CurrentStageIndex => _currentStageIndex;
    public int TotalStages => _plantsInEachGrowingStage.Count;
    
    public int GetTotalStages()
    {
        return _plantsInEachGrowingStage.Count;
    }
}
