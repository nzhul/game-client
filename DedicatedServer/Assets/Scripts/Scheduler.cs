using Assets.Scripts.MessageHandlers;
using System.Linq;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("ClearRegions", 5, 5);
    }

    private void ClearRegions()
    {
        // Debug.Log("Executing 'CleanRegions' scheduled job!: " + Time.time);
        var activeRegionIds = NetworkServer.Instance.Regions.Keys.ToArray();
        foreach (var regionId in activeRegionIds)
        {
            bool anyUsersWithThisRegion = NetworkServer.Instance.Connections.Any(c => c.Value.RegionIds.Any(r => r == regionId));

            if (!anyUsersWithThisRegion)
            {
                Debug.Log($"Region with Id {regionId} unloaded!");
                NetworkServer.Instance.Regions.Remove(regionId);
                DisconnectEventHandler.InvokeRegionUnloadEvent(regionId);
            }
        }
    }
}
