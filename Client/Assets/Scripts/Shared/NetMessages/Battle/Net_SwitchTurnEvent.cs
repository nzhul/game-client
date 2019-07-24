using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

namespace Assets.Scripts.Shared.NetMessages.Battle
{
    [Serializable]
    public class Net_SwitchTurnEvent : NetMessage
    {
        public Net_SwitchTurnEvent()
        {
            this.OperationCode = NetOperationCode.SwitchTurnEvent;
        }

        public Guid BattleId { get; set; }

        public int CurrentPlayerId { get; set; }

        public Turn Turn { get; set; }
    }
}
