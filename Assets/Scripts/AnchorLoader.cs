using System;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor _anchorPrefab;
    private FloorPrefabPlacer _floorPrefabPlacer;
    [SerializeField] private CurrentProgress currentProgressUI;

    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onAnchorLoaded;

    private void Awake()
    {
        _floorPrefabPlacer = GetComponent<FloorPrefabPlacer>();
        _anchorPrefab = _floorPrefabPlacer.potAnchorPrefab;
        _onAnchorLoaded = OnLocalized;
    }

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            Debug.Log("[AnchorLoader] Not Success");
            return;
        }

        
        unboundAnchor.TryGetPose(out Pose pose);
        var spatialAnchor = Instantiate(_anchorPrefab, pose.position, pose.rotation);
        unboundAnchor.BindTo(spatialAnchor);
    
        string lastCreatedUuid = PlayerPrefs.GetString(FloorPrefabPlacer.LastCreatedAnchorUUIDKey, "");
    
        if (spatialAnchor.Uuid.ToString() == lastCreatedUuid)
        {
            Debug.Log("[AnchorLoader] This is the last created anchor, updating FloorPrefabPlacer reference");
            _floorPrefabPlacer.SetLastCreatedAnchor(spatialAnchor);
        }
    
        PlantGrowing plantGrowing = spatialAnchor.GetComponentInChildren<PlantGrowing>();
        if (plantGrowing != null)
        {
            plantGrowing.Reset();
            if (spatialAnchor.Uuid.ToString() == lastCreatedUuid)
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
