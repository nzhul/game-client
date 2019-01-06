namespace Assets.Scripts.Shared.NetMessages
{
    public class Net_OnConnectRequest : NetMessage
    {
        public Net_OnConnectRequest()
        {
            OperationCode = NetOperationCode.OnConnectRequest;
        }

        public int ConnectionId { get; set; }

        public byte Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}
