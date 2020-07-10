﻿using Assets.Scripts.Games;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.Battle;
using UnityEngine;

namespace Assets.Scripts.Network.Services.TCP
{
    public class BattleService : IBattleService
    {
        public void EndBattle(Battle battle, int winnerId)
        {
            if (winnerId <= 0)
            {
                // both players are loosers
                // send two defeat requests
            }

            if (battle.AttackerArmy.Id == winnerId)
            {
                // attacker is winner
                // defender is defeated
                // send win to attacker and defeat to defender
            }
            else if (battle.DefenderArmy.Id == winnerId)
            {
                // attacker is defeated
                // defender is winner
                // send defeat to attacker and win to defender
            }

            // DO API call to save battle outcome
        }

        public void NullifyUnitPoints(int gameId, int heroId, int unitId, bool isDefend)
        {
            var unit = GameManager.Instance.GetUnit(gameId, heroId);

            if (unit == null)
            {
                Debug.LogError("Cannot find unit with Id " + unitId);
                return;
            }

            unit.MovementPoints = 0;
            unit.ActionPoints = 0;

            if (isDefend)
            {
                // todo: temporary increase armor for 1 turn.
            }
        }

        public void SwitchTurn(Battle battle)
        {
            Debug.Log("Turn time expired - switching turns!");
            if (battle.Turn == Turn.Attacker)
            {
                battle.Turn = Turn.Defender;
                battle.CurrentUnitId = battle.DefenderArmyId;
            }
            else
            {
                battle.Turn = Turn.Attacker;
                battle.CurrentUnitId = battle.AttackerArmyId;
            }

            battle.LastTurnStartTime = Time.time;
            battle.Log.Add("Turn time expired - switching turns! New Player is: " + battle.CurrentUnitId);
            this.SendSwitchTurnEvent(battle);
        }

        private void SendSwitchTurnEvent(Battle battle)
        {
            Net_SwitchTurnEvent msg = new Net_SwitchTurnEvent();
            msg.BattleId = battle.Id;
            msg.CurrentUnitId = battle.CurrentUnitId;
            msg.Turn = battle.Turn;

            bool shouldNotifyAttacker = false;
            bool shouldNotifyDefender = false;

            switch (battle.BattleScenario)
            {
                case BattleScenario.HUvsAI:
                    shouldNotifyAttacker = true;
                    shouldNotifyDefender = false;
                    break;
                case BattleScenario.AIvsAI:
                    shouldNotifyAttacker = false;
                    shouldNotifyDefender = false;
                    break;
                case BattleScenario.HUvsHU:
                    shouldNotifyAttacker = true;
                    shouldNotifyDefender = true;
                    break;
                case BattleScenario.AIvsHU:
                    shouldNotifyAttacker = false;
                    shouldNotifyDefender = true;
                    break;
                case BattleScenario.Unknown:
                    break;
                default:
                    break;
            }

            if (shouldNotifyAttacker)
            {
                NetworkServer.Instance.SendClient(0, battle.AttackerConnectionId, msg);
            }

            if (shouldNotifyDefender)
            {
                NetworkServer.Instance.SendClient(0, battle.DefenderConnectionId, msg);
            }
        }
    }
}