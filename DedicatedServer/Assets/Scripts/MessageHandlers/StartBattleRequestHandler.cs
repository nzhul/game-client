using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
{
    public class StartBattleRequestHandler : IMessageHandler
    {
        public static event Action<Battle> OnBattleStarted;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_StartBattleRequest msg = (Net_StartBattleRequest)input;
            Net_OnStartBattle rmsg = new Net_OnStartBattle();

            if (msg.IsValid())
            {
                BattleScenario scenario = this.ResolveBattleScenario(msg.AttackerType, msg.DefenderType);
                rmsg.Success = 1;
                rmsg.AttackerId = msg.AttackerId;
                rmsg.DefenderId = msg.DefenderId;
                rmsg.BattleScenario = scenario;

                // 1. Add new record into NetworkServer.Instance.ActiveBattles
                // 2. Send OnStartBattle back to the client.

                Battle newBattle = new Battle()
                {
                    Id = Guid.NewGuid(),
                    AttackerId = msg.AttackerId,
                    DefenderId = msg.DefenderId,
                    AttackerHero = NetworkServer.Instance.GetHeroById(msg.AttackerId),
                    DefenderHero = NetworkServer.Instance.GetHeroById(msg.DefenderId),
                    CurrentHeroId = msg.AttackerId,
                    AttackerType = msg.AttackerType,
                    DefenderType = msg.DefenderType,
                    BattleScenario = scenario,
                    LastTurnStartTime = Time.time
                };

                newBattle.SelectedUnit = NetworkServer.Instance.GetRandomUnit(newBattle.CurrentHeroId);

                this.UpdateUnitsData(newBattle.AttackerHero);
                this.UpdateUnitsData(newBattle.DefenderHero);

                rmsg.BattleId = newBattle.Id;
                rmsg.SelectedUnitId = newBattle.SelectedUnit.Id;

                this.ConfigurePlayerReady(newBattle, scenario);
                newBattle.AttackerConnectionId = NetworkServer.Instance.GetConnectionIdByHeroId(newBattle.AttackerId);
                newBattle.DefenderConnectionId = NetworkServer.Instance.GetConnectionIdByHeroId(newBattle.DefenderId);

                NetworkServer.Instance.ActiveBattles.Add(newBattle);
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
                OnBattleStarted?.Invoke(newBattle);
            }
        }

        private void UpdateUnitsData(Hero hero)
        {
            //TODO apply upgrades before the battle!
            foreach (var unit in hero.Units)
            {
                var config = NetworkServer.Instance.UnitConfigurations[unit.CreatureType];
                unit.MovementPoints = config.MovementPoints;
                unit.MaxMovementPoints = unit.MovementPoints;
                unit.ActionPoints = config.ActionPoints;
                unit.MaxMovementPoints = unit.ActionPoints;
                unit.MinDamage = config.MinDamage;
                unit.MaxDamage = config.MaxDamage;
                unit.Hitpoints = config.Hitpoints;
                unit.BaseHitpoints = unit.Hitpoints;
                unit.MaxHitpoints = unit.Hitpoints;
                unit.Mana = config.Mana;
                unit.Armor = config.Armor;
                unit.AttackType = config.AttackType;
                unit.ArmorType = config.ArmorType;
                unit.Level = config.CreatureLevel;
            }
        }

        private void ConfigurePlayerReady(Battle newBattle, BattleScenario scenario)
        {
            switch (scenario)
            {
                case BattleScenario.HUvsAI:
                    newBattle.AttackerReady = false;
                    newBattle.DefenderReady = true;
                    break;
                case BattleScenario.AIvsAI:
                    newBattle.AttackerReady = true;
                    newBattle.DefenderReady = true;
                    break;
                case BattleScenario.HUvsHU:
                    newBattle.AttackerReady = false;
                    newBattle.DefenderReady = false;
                    break;
                case BattleScenario.AIvsHU:
                    newBattle.AttackerReady = true;
                    newBattle.DefenderReady = false;
                    break;
                default:
                    break;
            }
        }

        private BattleScenario ResolveBattleScenario(PlayerType attackerType, PlayerType defenderType)
        {
            if (attackerType == PlayerType.Human && defenderType == PlayerType.AI)
            {
                return BattleScenario.HUvsAI;
            }
            else if (attackerType == PlayerType.AI && defenderType == PlayerType.AI)
            {
                return BattleScenario.AIvsAI;
            }
            else if (attackerType == PlayerType.Human && defenderType == PlayerType.Human)
            {
                return BattleScenario.HUvsHU;
            }
            else if (attackerType == PlayerType.AI && defenderType == PlayerType.Human)
            {
                return BattleScenario.AIvsHU;
            }

            return BattleScenario.Unknown;
        }
    }

    // # Battle Scenarios
    // 1. HU vs AI Monster
    // 2. AI Human vs AI Monster (AutoCombat)
    // 3. Human vs Human (PvP)

    // # Not supported scenarios
    // 1. AI HU vs AI HU (Autocombat)
    // 2. HU vs AI HU (one of the players is Autocombat)
}
