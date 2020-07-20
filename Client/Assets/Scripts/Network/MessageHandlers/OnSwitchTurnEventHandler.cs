using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Shared.NetMessages.Battle;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnSwitchTurnEventHandler : IMessageHandler
    {
        public static event Action<Net_SwitchTurnEvent> OnSwitchTurn;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // 1. Raise event
            var msg = (Net_SwitchTurnEvent)input;

            Debug.Log($"Switching turns: New Player is {msg.Turn} with Unit: {msg.CurrentUnitId}");

            BattleData bd = DataManager.Instance.BattleData;
            //bd.CurrentArmyId = msg.CurrentUnitId;
            //bd.SelectedUnit = 
            bd.LastTurnStartTime = Time.time;
            bd.RemainingTimeForThisTurn = BattleManager.TURN_DURATION;
            bd.Turn = msg.Turn;

            bd.ActionsEnabled = this.IsPlayerUnit(bd, msg.CurrentUnitId);

            // Raise "YOUR TURN" notification for the CurrentPlayer!

            OnSwitchTurn?.Invoke(msg);
        }

        private bool IsPlayerUnit(BattleData bd, int currentUnitId)
        {
            return bd.PlayerArmy.Units.Any(x => x.Id == currentUnitId);
        }
    }
}
