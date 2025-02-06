using System.Collections.Generic;
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
        if (_currentStageIndex > 0) _plantsInEachGrowingStage[_currentStageIndex - 1].SetActive(false);
        _plantsInEachGrowingStage[_currentStageIndex].SetActive(true);
        if (_currentStageIndex < _plantsInEachGrowingStage.Count) _currentStageIndex++;
    }
}
