using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor anchorPrefab;
    private FloorPrefabPlacer floorPrefabPlacer;
    [SerializeField] private CurrentProgress currentProgressUI;

    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onAnchorLoaded;

    private void Awake()
    {
        floorPrefabPlacer = GetComponent<FloorPrefabPlacer>();
        anchorPrefab = floorPrefabPlacer.potAnchorPrefab;
        _onAnchorLoaded = onLocalized;
    }

    private void onLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        Debug.Log($"[AnchorLoader] onLocalized called for anchor {unboundAnchor.Uuid}, success: {success}");
    
        if (!success)
        {
            Debug.LogError($"[AnchorLoader] Failed to localize anchor {unboundAnchor.Uuid}");
            return;
        }

        var pose = unboundAnchor.Pose;
        var spatialAnchor = Instantiate(anchorPrefab, pose.position, pose.rotation);
        Debug.Log($"[AnchorLoader] Instantiated new anchor at position {pose.position}");
    
        unboundAnchor.BindTo(spatialAnchor);
    
        string lastCreatedUUID = PlayerPrefs.GetString(FloorPrefabPlacer.LastCreatedAnchorUUIDKey, "");
        Debug.Log($"[AnchorLoader] Last created UUID: {lastCreatedUUID}");
    
        // If this is the last created anchor, update FloorPrefabPlacer
        if (spatialAnchor.Uuid.ToString() == lastCreatedUUID)
        {
            Debug.Log("[AnchorLoader] This is the last created anchor, updating FloorPrefabPlacer reference");
            floorPrefabPlacer.SetLastCreatedAnchor(spatialAnchor);
        }
    
        PlantGrowing plantGrowing = spatialAnchor.GetComponentInChildren<PlantGrowing>();
        if (plantGrowing != null)
        {
            plantGrowing.Reset();
            if (spatialAnchor.Uuid.ToString() == lastCreatedUUID)
            {
                string progressKey = "PlantProgress_" + spatialAnchor.Uuid.ToString();
                int savedProgress = PlayerPrefs.GetInt(progressKey, 0);
                
                currentProgressUI.SetProgress(savedProgress, 3);
                    for (int i = 0; i < savedProgress; i++)
                    {
                        plantGrowing.AdvanceToNextStage();
                    }
                Debug.Log($"[AnchorLoader] Loading progress for active plant: {savedProgress}");
            }
            else
            {
                Debug.Log("[AnchorLoader] Setting plant to full grown state");
                plantGrowing.SetFullGrownState();
            }
        }
        else
        {
            Debug.LogWarning("[AnchorLoader] No PlantGrowing component found on the instantiated anchor");
        }
    }

    public void LoadAnchorByUuid()
    {
        if (!PlayerPrefs.HasKey(FloorPrefabPlacer.NumUuidsPlayerPref))
        {
            PlayerPrefs.SetInt(FloorPrefabPlacer.NumUuidsPlayerPref, 0);
        }
        
        var playerUuidCount = PlayerPrefs.GetInt(FloorPrefabPlacer.NumUuidsPlayerPref);
        
        if(playerUuidCount == 0)
            return;

        var uuids = new Guid[playerUuidCount];

        for (int i = 0; i < playerUuidCount; ++i)
        {
            var uuidKey = "uuid" + i;
            var currentPlayerUuid = PlayerPrefs.GetString(uuidKey);
            uuids[i] = new Guid(currentPlayerUuid);
        }

        Load(new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 10,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids
        });
    }

    private void Load(OVRSpatialAnchor.LoadOptions loadOptions)
    {
        Debug.Log("[AnchorLoader] Load method: Loading anchors from " + loadOptions.StorageLocation);
        OVRSpatialAnchor.LoadUnboundAnchors(loadOptions, anchors =>
        {
            if (anchors == null)
                return;
            foreach (var anchor in anchors)
            {
                if (anchor.Localized)
                {
                    _onAnchorLoaded(anchor, true);
                }else if (!anchor.Localized)
                {
                    anchor.Localize(_onAnchorLoaded);
                }
            }
        });
    }
    
}
