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
    }

    public void AdvanceToNextStage()
    {
        if (!_isAlive || currentStageIndex >= _plantsInEachGrowingStage.Count) return;
        StartCoroutine(AsyncAdvance());
    }

    private IEnumerator AsyncAdvance()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("appear");
        }

        if (currentStageIndex > 0)
        {
            _plantsInEachGrowingStage[currentStageIndex - 1].SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (currentStageIndex < _plantsInEachGrowingStage.Count)
        {
            _plantsInEachGrowingStage[currentStageIndex].SetActive(true);
        }
    
        currentStageIndex++;
    
        if (_vfxGrowing != null && _vfxTransform != null)
        {
            Instantiate(_vfxGrowing, _vfxTransform.position, _vfxTransform.rotation);
        }
    
        yield break;
    }

    public void Wither()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("wither");
        }
        _isAlive = false;
        if (_vfxWithering != null && _vfxTransform != null)
        {
            Instantiate(_vfxWithering, _vfxTransform.position, _vfxTransform.rotation, _vfxTransform);
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        if (_growingEffect != null)
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