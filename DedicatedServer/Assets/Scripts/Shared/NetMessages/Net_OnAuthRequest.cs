using System;

namespace Assets.Scripts.Shared.NetMessages
{
    [Serializable]
    public class Net_OnAuthRequest : NetMessage
    {
        public Net_OnAuthRequest()
        {
            OperationCode = NetOperationCode.OnAuthRequest;
        }

        public int ConnectionId { get; set; }

        public byte Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}
