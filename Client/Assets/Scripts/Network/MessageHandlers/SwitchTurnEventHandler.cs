using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Shared.NetMessages.Battle;
using System;
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

            Debug.Log("Switching turns: New Player is " + msg.CurrentPlayerId);

            BattleData bd = DataManager.Instance.BattleData;
            bd.CurrentPlayerId = msg.CurrentPlayerId;
            bd.LastTurnStartTime = Time.time;
            bd.RemainingTimeForThisTurn = BattleManager.TURN_DURATION;
            bd.Turn = msg.Turn;

            bd.ActionsEnabled = BattleManager.Instance.CurrentPlayerIsMe(msg.CurrentPlayerId) ? true : false;

            // Raise "YOUR TURN" notification for the CurrentPlayer!

            OnSwitchTurn?.Invoke(msg);
        }
    }
}
