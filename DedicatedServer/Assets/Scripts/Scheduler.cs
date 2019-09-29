using Assets.Scripts.MessageHandlers;
using Assets.Scripts.Services;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scheduler : MonoBehaviour
{
    private const int TURN_DURATION = 20; // seconds
    private const int IDLE_TIMEOUT = (TURN_DURATION * 2) + (TURN_DURATION / 2); // seconds -> 20 * 2 + 20 / 2 = 40 + 10 = 50
    private IBattleService battleService;

    private void Start()
    {
        this.battleService = new BattleService();
        InvokeRepeating("ClearRegions", 5, 5);
        InvokeRepeating("SwitchBattleTurns", 5, 1);
    }

    private void SwitchBattleTurns()
    {
        var completedBattles = new List<Battle>();

        foreach (var battle in NetworkServer.Instance.ActiveBattles)
        {
            if (battle.State != BattleState.Fight)
            {
                continue;
            }

            DateTime idleTime = DateTime.UtcNow.AddSeconds(-IDLE_TIMEOUT);

            if (battle.AttackerLastActivity < idleTime && battle.DefenderLastActivity < idleTime)
            {
                this.battleService.EndBattle(battle, -1);
                completedBattles.Add(battle);
            }

            if (battle.LastTurnStartTime + TURN_DURATION < Time.time)
            {
                this.battleService.SwitchTurn(battle);
            }
        }

        foreach (var battle in completedBattles)
        {
            Debug.Log($"Ending Idle battle: AttackerId: {battle.AttackerHero.Id}, Defender: {battle.DefenderHero.Id}, BattleId: {battle.Id}");
            UIManager.Instance.OnBattleEnded(battle.Id);
            NetworkServer.Instance.ActiveBattles.Remove(battle);
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
