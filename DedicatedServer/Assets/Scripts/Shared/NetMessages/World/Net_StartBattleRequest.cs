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

        public int HeroId { get; set; }

        public int MonsterId { get; set; }

        public bool IsValid()
        {
            bool result = true;

            if (this.HeroId == 0)
            {
                return false;
            }

            if (this.MonsterId == 0)
            {
                return false;
            }

            return result;
        }
    }
}
