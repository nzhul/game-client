using System;
using System.Linq;
using Assets.Scripts.Games;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class StartBattleRequestHandler : IMessageHandler
    {
        public static event Action<Battle> OnBattleStarted;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_StartBattleRequest msg = (Net_StartBattleRequest)input;
            Net_OnStartBattle rmsg = new Net_OnStartBattle();
            var game = GameManager.Instance.GetGameByConnectionId(connectionId);

            if (msg.IsValid())
            {
                BattleScenario scenario = this.ResolveBattleScenario(msg.AttackerType, msg.DefenderType);
                rmsg.AttackerArmyId = msg.AttackerArmyId;
                rmsg.DefenderArmyId = msg.DefenderArmyId;
                rmsg.BattleScenario = scenario;

                // 1. Add new record into NetworkServer.Instance.ActiveBattles
                // 2. Send OnStartBattle back to the client.

                Battle newBattle = new Battle()
                {
                    Id = Guid.NewGuid(),
                    GameId = game.Id,
                    AttackerArmyId = msg.AttackerArmyId,
                    DefenderArmyId = msg.DefenderArmyId,
                    AttackerArmy = game.Armies.FirstOrDefault(x => x.Id == msg.AttackerArmyId),
                    DefenderArmy = game.Armies.FirstOrDefault(x => x.Id == msg.DefenderArmyId),
                    AttackerType = msg.AttackerType,
                    DefenderType = msg.DefenderType,
                    BattleScenario = scenario,
                    LastTurnStartTime = Time.time
                };

                newBattle.SelectedUnit = GameManager.Instance.GetRandomAvailibleUnit(newBattle.AttackerArmy);

                this.UpdateUnitsData(newBattle.AttackerArmy);
                this.UpdateUnitsData(newBattle.DefenderArmy);

                rmsg.BattleId = newBattle.Id;
                rmsg.SelectedUnitId = newBattle.SelectedUnit.Id;
                rmsg.Turn = Turn.Attacker;

                this.ConfigurePlayerReady(newBattle, scenario);
                newBattle.AttackerConnectionId = GameManager.Instance.GetConnectionIdByArmyId(game.Id, newBattle.AttackerArmyId);
                newBattle.DefenderConnectionId = GameManager.Instance.GetConnectionIdByArmyId(game.Id, newBattle.DefenderArmyId);

                NetworkServer.Instance.ActiveBattles.Add(newBattle);
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);

                RequestManagerHttp.BattleService.RegisterBattle(newBattle.Id, NetworkServer.Instance.Connections[connectionId].UserId);
                // TODO: Register battle for other player when pvp battle.

                OnBattleStarted?.Invoke(newBattle);
            }
        }

        private void UpdateUnitsData(Army Army)
        {
            //TODO apply upgrades before the battle!
            foreach (var unit in Army.Units)
            {
                var config = NetworkServer.Instance.UnitConfigurations[unit.Type];
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
