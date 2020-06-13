using System;

namespace Assets.Scripts.Network.Shared.NetMessages.World.ClientServer
{
    [Serializable]
    public class Net_TeleportRequest : NetMessage
    {
        public Net_TeleportRequest()
        {
            OperationCode = NetOperationCode.TeleportRequest;
        }

        public int HeroId { get; set; }

        public int RegionId { get; set; }

        public int DwellingId { get; set; }
    }
}