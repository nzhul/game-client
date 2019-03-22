using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

namespace Assets.Scripts.Shared.NetMessages.World
{
    [Serializable]
    public class Net_MapMovementRequest : NetMessage
    {
        public Net_MapMovementRequest()
        {
            OperationCode = NetOperationCode.MapMovementRequest;
        }

        public int HeroId { get; set; }

        public int RegionId { get; set; }

        //public int NewX { get; set; }

        //public int NewY { get; set; }

        public Coord Destination { get; set; }
    }
}


//[Serializable]
//public class Net_WorldEnterRequest : NetMessage
//{
//    public Net_WorldEnterRequest()
//    {
//        OperationCode = NetOperationCode.WorldEnterRequest;
//    }

//    public int UserId { get; set; }

//    public int CurrentRealmId { get; set; }

//    public int[] RegionsForLoading { get; set; }
//}