using Assets.Scripts.Shared.NetMessages.Battle;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using Assets.Scripts.Shared.NetMessages.World.Models;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class BattleService : IBattleService
    {
        public void SwitchTurn(Battle battle)
        {
            Debug.Log("Turn time expired - switching turns!");
            if (battle.Turn == Turn.Attacker)
            {
                battle.Turn = Turn.Defender;
                battle.CurrentPlayerId = battle.DefenderId;
            }
            else
            {
                battle.Turn = Turn.Attacker;
                battle.CurrentPlayerId = battle.AttackerId;
            }

            battle.LastTurnStartTime = Time.time;
            battle.Log.Add("Turn time expired - switching turns! New Player is: " + battle.CurrentPlayerId);
            this.SendSwitchTurnEvent(battle);
        }

        private void SendSwitchTurnEvent(Battle battle)
        {
            Net_SwitchTurnEvent msg = new Net_SwitchTurnEvent();
            msg.BattleId = battle.Id;
            msg.CurrentPlayerId = battle.CurrentPlayerId;
            msg.Turn = battle.Turn;

            bool shouldNotifyAttacker = false;
            bool shouldNotifyDefender = false;

            switch (battle.BattleScenario)
            {
                case BattleScenario.HUvsMonsterAI:
                    shouldNotifyAttacker = true;
                    shouldNotifyDefender = false;
                    break;
                case BattleScenario.HUAIvsMonsterAI:
                    shouldNotifyAttacker = false;
                    shouldNotifyDefender = false;
                    break;
                case BattleScenario.HUvsHU:
                    shouldNotifyAttacker = true;
                    shouldNotifyDefender = true;
                    break;
                case BattleScenario.MonsterAIvsHU:
                    shouldNotifyAttacker = false;
                    shouldNotifyDefender = true;
                    break;
                case BattleScenario.MonsterAIvsHUAI:
                    shouldNotifyAttacker = false;
                    shouldNotifyDefender = false;
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
