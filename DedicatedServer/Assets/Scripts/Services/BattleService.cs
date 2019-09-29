using Assets.Scripts.Shared.NetMessages.Battle;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using Assets.Scripts.Shared.NetMessages.World.Models;
using UnityEngine;

namespace Assets.Scripts.Services
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

            if (battle.AttackerHero.Id == winnerId)
            {
                // attacker is winner
                // defender is defeated
                // send win to attacker and defeat to defender
            }
            else if (battle.DefenderHero.Id == winnerId)
            {
                // attacker is defeated
                // defender is winner
                // send defeat to attacker and win to defender
            }

            // DO API call to save battle outcome
        }

        public void NullifyHeroPoints(int requesterHeroId, bool isDefend)
        {
            var hero = NetworkServer.Instance.GetHeroById(requesterHeroId);

            if (hero == null)
            {
                Debug.LogError("Cannot find hero with Id " + requesterHeroId);
                return;
            }

            hero.MovementPoints = 0;
            hero.ActionPoints = 0;

            if (isDefend)
            {
                // todo: temporary increase armor for 1 turn.
            }
        }

        public void NullifyUnitPoints(int heroId, int unitId, bool isDefend)
        {
            var unit = NetworkServer.Instance.GetUnitById(heroId, unitId);

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
                battle.CurrentHeroId = battle.DefenderId;
            }
            else
            {
                battle.Turn = Turn.Attacker;
                battle.CurrentHeroId = battle.AttackerId;
            }

            battle.LastTurnStartTime = Time.time;
            battle.Log.Add("Turn time expired - switching turns! New Player is: " + battle.CurrentHeroId);
            this.SendSwitchTurnEvent(battle);
        }

        private void SendSwitchTurnEvent(Battle battle)
        {
            Net_SwitchTurnEvent msg = new Net_SwitchTurnEvent();
            msg.BattleId = battle.Id;
            msg.CurrentPlayerId = battle.CurrentHeroId;
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
