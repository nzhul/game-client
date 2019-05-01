using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

namespace Assets.Scripts.Shared.NetMessages.World
{
    [Serializable]
    public class Net_OnMapMovement : NetMessage
    {
        public Net_OnMapMovement()
        {
            OperationCode = NetOperationCode.OnMapMovement;
        }

        public string Error { get; set; }

        public byte Success { get; set; }

        public int HeroId { get; set; }

        //public int NewX { get; set; }

        //public int NewY { get; set; }

        public Coord Destination { get; set; }
    }
}