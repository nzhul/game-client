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

        public int HeroId { get; set; }

        public int MonsterId { get; set; }

        public string Error { get; set; }

        public byte Success { get; set; }
    }
}
