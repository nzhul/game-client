using System;

namespace Assets.Scripts.Shared.NetMessages.World.ClientServer
{
    [Serializable]
    public class Net_CancelFindOpponentRequest : NetMessage
    {
        public Net_CancelFindOpponentRequest()
        {
            OperationCode = NetOperationCode.CancelFindOpponentRequest;
        }
    }
}
