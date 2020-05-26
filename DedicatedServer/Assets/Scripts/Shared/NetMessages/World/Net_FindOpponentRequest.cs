namespace Assets.Scripts.Shared.NetMessages.World
{
    public class Net_FindOpponentRequest : NetMessage
    {
        public Net_FindOpponentRequest()
        {
            OperationCode = NetOperationCode.FindOpponentRequest;
        }

        public int UserId { get; set; }

        public string Faction { get; set; }

        public string Class { get; set; }
    }
}
