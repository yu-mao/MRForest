using System;
using System.Collections;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class FloorPrefabPlacer : MonoBehaviour
{
    // Constants
    public const string NumUuidsPlayerPref = "NumUuids";
    public const string IsCurrentPlantGrowingPref = "IsCurrentPlantGrowing";

    // Prefab Reference
    public OVRSpatialAnchor potAnchorPrefab;

    // Private fields
    private List<OVRSpatialAnchor> anchors = new List<OVRSpatialAnchor>();
    private OVRSpatialAnchor lastCreatedAnchor;
    private bool isInitialized;
    private bool isCurrentPlantGrowing;
    private AnchorLoader anchorLoader;
    
    

    private void Awake()
    {
        isCurrentPlantGrowing = PlayerPrefs.GetInt(IsCurrentPlantGrowingPref, 0) == 1;
        anchorLoader = GetComponent<AnchorLoader>();
        LoadSavedAnchors();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            SetCurrentPlantGrowing(false);
            UnsaveAllAnchors();
        }

        if (!isInitialized || isCurrentPlantGrowing)
        {
            return;
        }

        Vector3 rayOrigin = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 rayDirection = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch) * Vector3.forward;

        if (MRUK.Instance?.GetCurrentRoom()?.Raycast(
            new Ray(rayOrigin, rayDirection),
            Mathf.Infinity,
            new LabelFilter(MRUKAnchor.SceneLabels.FLOOR),
            out RaycastHit hit,
            out MRUKAnchor anchorHit) == true)
        {
            if (anchorHit != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                SetCurrentPlantGrowing(true);
                CreateSpatialAnchor(hit.point, rotation);
            }
        }
    }

    public void Initialized() => isInitialized = true;

    
    public  void PlantGrown() => SetCurrentPlantGrowing(false);

    public bool GetCurrentPlantStatus()
    {
        return isCurrentPlantGrowing;
    }
    
    public void CreateSpatialAnchor(Vector3 position, Quaternion rotation)
    {
        OVRSpatialAnchor spatialAnchor = Instantiate(potAnchorPrefab, position, rotation);
        lastCreatedAnchor = spatialAnchor;
        StartCoroutine(AnchorCreated(spatialAnchor));
        PlantGrowing plantGrowing = GameObject.FindObjectOfType<PlantGrowing>();
        if (plantGrowing != null)
        {
            plantGrowing.Initialize();
            PlayerPrefs.SetInt("PlantProgress", 0);
            PlayerPrefs.Save();
        }
    }

    private void LoadSavedAnchors()
    {
        anchorLoader.LoadAnchorByUuid();
    }

    private void UnsaveAllAnchors()
    {
        SetCurrentPlantGrowing(false);
        foreach (var anchor in anchors)
        {
            UnsaveAnchor(anchor);
        }
        anchors.Clear();
        ClearAllUuidsFromPlayerprefs();
    }

    private void ClearAllUuidsFromPlayerprefs()
    {
        if (PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            for (int i = 0; i < PlayerPrefs.GetInt(NumUuidsPlayerPref); i++)
            {
                PlayerPrefs.DeleteKey($"uuid{i}");
                Debug.Log($"Anchor Uuid{i} deleted");
            }
            PlayerPrefs.DeleteKey(NumUuidsPlayerPref);
        }
        
    }

    private void UnsaveAnchor(OVRSpatialAnchor anchor)
    {
        anchor.Erase((erasedAnchor, success) =>
        {
            Debug.Log($"Anchor erased: {success}");
        });
    }

    private IEnumerator AnchorCreated(OVRSpatialAnchor spatialAnchor)
    {
        while (!spatialAnchor.Created && !spatialAnchor.Localized)
        {
            yield return new WaitForEndOfFrame();
        }

        Guid anchorUuid = spatialAnchor.Uuid;
        anchors.Add(spatialAnchor);
        
        spatialAnchor.Save((savedAnchor, success) =>
        {
            Debug.Log($"Created anchor Saved {anchorUuid}");
        });
        
        SaveUuidToPlayerPrefs(anchorUuid);
        lastCreatedAnchor = spatialAnchor;
    }

    private void SaveUuidToPlayerPrefs(Guid anchorUuid)
    {
        if (!PlayerPrefs.HasKey(NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(NumUuidsPlayerPref, 0);
        }

        int playerNumUuids = PlayerPrefs.GetInt(NumUuidsPlayerPref);
        PlayerPrefs.SetString($"uuid{playerNumUuids}", anchorUuid.ToString());
        PlayerPrefs.SetInt(NumUuidsPlayerPref, ++playerNumUuids);
        PlayerPrefs.Save();
    }

    public void SetCurrentPlantGrowing(bool state)
    {
        isCurrentPlantGrowing = state;
        PlayerPrefs.SetInt(IsCurrentPlantGrowingPref, state ? 1 : 0);
        PlayerPrefs.Save();
    }
}