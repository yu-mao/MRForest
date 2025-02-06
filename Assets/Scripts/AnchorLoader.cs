using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnchorLoader : MonoBehaviour
{
    private OVRSpatialAnchor anchorPrefab;
    private FloorPrefabPlacer floorPrefabPlacer;
    
    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onAnchorLoaded;

    private void Awake()
    {
        floorPrefabPlacer = GetComponent<FloorPrefabPlacer>();
        anchorPrefab = floorPrefabPlacer.potAnchorPrefab;
        _onAnchorLoaded = onLocalized;
    }

    private void onLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success) return;
        
        var pose = unboundAnchor.Pose;
        var spatialAnchor = Instantiate(anchorPrefab, pose.position, pose.rotation);
        unboundAnchor.BindTo(spatialAnchor);

        if (spatialAnchor.TryGetComponent<OVRSpatialAnchor>(out var anchor))
        {
            Debug.Log(anchor.name + " is bound to " + spatialAnchor.name);
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
            Timeout = 5,
            StorageLocation = OVRSpace.StorageLocation.Local,
            Uuids = uuids
        });
    }

    private void Load(OVRSpatialAnchor.LoadOptions loadOptions)
    {
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
