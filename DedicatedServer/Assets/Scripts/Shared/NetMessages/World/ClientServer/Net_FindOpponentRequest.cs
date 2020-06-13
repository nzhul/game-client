using System;
using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Shared.NetMessages.World.ClientServer
{
    [Serializable]
    public class Net_FindOpponentRequest : NetMessage
    {
        public Net_FindOpponentRequest()
        {
            OperationCode = NetOperationCode.FindOpponentRequest;
        }

        public HeroClass Class { get; set; }

        public bool IsValid()
        {
            return true;
        }
    }
}
