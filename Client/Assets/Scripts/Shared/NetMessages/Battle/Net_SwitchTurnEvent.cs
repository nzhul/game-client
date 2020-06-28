using System;
using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Shared.NetMessages.Battle
{
    [Serializable]
    public class Net_SwitchTurnEvent : NetMessage
    {
        public Net_SwitchTurnEvent()
        {
            this.OperationCode = NetOperationCode.OnSwitchTurn;
        }

        public Guid BattleId { get; set; }

        public int CurrentUnitId { get; set; }

        public Turn Turn { get; set; }
    }
}
