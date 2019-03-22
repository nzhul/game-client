using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

namespace Assets.Scripts.Shared.NetMessages.World
{
    [Serializable]
    public class Net_OnTeleport : NetMessage
    {
        public Net_OnTeleport()
        {
            OperationCode = NetOperationCode.OnTeleport;
        }

        public string Error { get; set; }

        public byte Success { get; set; }

        public int HeroId { get; set; }

        public Coord Destination { get; set; }

        public int RegionId { get; set; }

        public int DwellingId { get; set; }
    }
}