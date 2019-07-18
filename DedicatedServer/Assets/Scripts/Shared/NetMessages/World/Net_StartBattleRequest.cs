using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

namespace Assets.Scripts.Shared.NetMessages.World
{
    [Serializable]
    public class Net_StartBattleRequest : NetMessage
    {
        public Net_StartBattleRequest()
        {
            OperationCode = NetOperationCode.StartBattleRequest;
        }

        public int AttackerId { get; set; }

        public int DefenderId { get; set; }

        public PlayerType AttackerType { get; set; }

        public PlayerType DefenderType { get; set; }

        public bool IsValid()
        {
            bool result = true;

            if (this.AttackerId == 0)
            {
                return false;
            }

            if (this.DefenderId == 0)
            {
                return false;
            }

            return result;
        }
    }
}
