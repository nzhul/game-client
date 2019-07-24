using Assets.Scripts.MessageHandlers;
using Assets.Scripts.Services;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using System.Linq;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    private const int TURN_DURATION = 20; // seconds
    private IBattleService battleService;

    private void Start()
    {
        this.battleService = new BattleService();
        InvokeRepeating("ClearRegions", 5, 5);
        InvokeRepeating("SwitchBattleTurns", 5, 1);
    }

    private void SwitchBattleTurns()
    {
        foreach (var battle in NetworkServer.Instance.ActiveBattles)
        {
            if (battle.State != BattleState.Fight)
            {
                continue;
            }

            if (battle.LastTurnStartTime + TURN_DURATION < Time.time)
            {
                this.battleService.SwitchTurn(battle);
            }
            //else
            //{
            //    Debug.Log("Remaining: " + ((battle.LastTurnStartTime + TURN_DURATION) - Time.time).ToString());
            //}
        }
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
