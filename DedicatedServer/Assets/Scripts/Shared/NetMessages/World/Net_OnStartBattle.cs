using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

namespace Assets.Scripts.Shared.NetMessages.World
{
    [Serializable]
    public class Net_OnStartBattle : NetMessage
    {
        public Net_OnStartBattle()
        {
            this.OperationCode = NetOperationCode.OnStartBattle;
        }

        public Guid BattleId { get; set; }

        public int AttackerId { get; set; }

        public int DefenderId { get; set; }

        public int SelectedUnitId { get; set; }

        public PlayerType AttackerType { get; set; }

        public PlayerType DefenderType { get; set; }

        public BattleScenario BattleScenario { get; set; }

        public string Error { get; set; }

        public byte Success { get; set; }
    }
}
