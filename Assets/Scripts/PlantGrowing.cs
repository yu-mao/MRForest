using System;
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
    
    [SerializeField] private GameObject _growingEffect;
    
    private Animator _animator;
    private bool _isAlive = true;
    

    public void Reset()
    {
        Debug.Log("[PlantGrowing] Reset called.");
        foreach (var plant in _plantsInEachGrowingStage)
        {
            plant.SetActive(false);
        }
        currentStageIndex = 0;
        _isAlive = true;
        
        if (_animator != null)
        {
            _animator.SetTrigger("reset");
        }
        else
        {
            Debug.LogError("[PlantGrowing] Cannot reset because _animator is null.");
        }
    }

    public void AdvanceToNextStage()
    {
        if (!_isAlive) return; 
        
        if (currentStageIndex >= _plantsInEachGrowingStage.Count) return;
        
        StartCoroutine(AsyncAdvance());
    }

    private IEnumerator AsyncAdvance()
    {
        Debug.Log($"[AsyncAdvance] Starting. currentStageIndex = {currentStageIndex}");
    
        if (_animator == null)
        {
            Debug.LogError("[AsyncAdvance] Animator is null! Make sure the Animator component is attached.");
        }
        else
        {
            _animator.SetTrigger("appear");
            Debug.Log("[AsyncAdvance] Trigger 'appear' set on animator.");
        }

        if (currentStageIndex > 0)
        {
            Debug.Log($"[AsyncAdvance] Deactivating previous stage at index {currentStageIndex - 1}");
            _plantsInEachGrowingStage[currentStageIndex - 1].SetActive(false);
        }
        else
        {
            Debug.Log("[AsyncAdvance] First stage: waiting 0.5 seconds to avoid animation glitch.");
            yield return new WaitForSeconds(0.5f);
        }

        // Check if the list contains an element at the current index.
        if (currentStageIndex < _plantsInEachGrowingStage.Count)
        {
            Debug.Log($"[AsyncAdvance] Activating stage at index {currentStageIndex}");
            _plantsInEachGrowingStage[currentStageIndex].SetActive(true);
        }
        else
        {
            Debug.LogError($"[AsyncAdvance] currentStageIndex ({currentStageIndex}) is out of range! List count: {_plantsInEachGrowingStage.Count}");
        }
    
        currentStageIndex++;
        Debug.Log($"[AsyncAdvance] Incremented currentStageIndex to {currentStageIndex}");
    
        // Instantiate VFX at the specified transform position/rotation
        if (_vfxGrowing != null && _vfxTransform != null)
        {
            Instantiate(_vfxGrowing, _vfxTransform.position, _vfxTransform.rotation);
            Debug.Log("[AsyncAdvance] Instantiated growing VFX.");
        }
        else
        {
            Debug.LogWarning("[AsyncAdvance] VFX growing prefab or transform is null.");
        }
    
        yield break;
    }

    public void Wither()
    {
        _animator.SetTrigger("wither");
        _isAlive = false;
        Instantiate(_vfxWithering, _vfxTransform);
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("[PlantGrowing] Animator is missing!");
        }
    }
    
    public int CurrentStageIndex => currentStageIndex;
    
    public int GetTotalStages()
    {
        return _plantsInEachGrowingStage.Count;
    }
    
    public void SetFullGrownState()
    {
        for (int i = 0; i < _plantsInEachGrowingStage.Count; i++)
        {
            _plantsInEachGrowingStage[i].SetActive(i == _plantsInEachGrowingStage.Count - 1);
        }
        _growingEffect.SetActive(false);
        currentStageIndex = _plantsInEachGrowingStage.Count;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            Wither();
        }
    }
}
