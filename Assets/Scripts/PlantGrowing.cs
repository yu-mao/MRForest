using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGrowing : MonoBehaviour
{
    public int currentStageIndex = 0;
    
    [SerializeField] private List<GameObject> _plantsInEachGrowingStage;
    [SerializeField] private List<GameObject> _vfxInEachGrowingStage;

    [SerializeField] private Transform _vfxTransform;
    [SerializeField] private GameObject _vfxGrowing;
    [SerializeField] private GameObject _vfxWithering;
    
    private Animator _animator;
    private bool _isAlive = true;

    public void Reset()
    {
        // hide all plants for later object pooling
        foreach (var plant in _plantsInEachGrowingStage)
        {
            plant.SetActive(false);
        }
        currentStageIndex = 0;
        _isAlive = true;
        _animator.SetTrigger("reset");
    }

    public void AdvanceToNextStage()
    {
        if (!_isAlive) return; 
        
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
        Instantiate(_vfxGrowing, _vfxTransform);

        yield break;
    }

    public void Wither()
    {
        _animator.SetTrigger("wither");
        _isAlive = false;
        Instantiate(_vfxWithering, _vfxTransform);
    }

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }
}
