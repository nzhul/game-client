﻿using System;
using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Shared.NetMessages.World.ServerClient
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

        public int ArmyId { get; set; }

        //public int NewX { get; set; }

        //public int NewY { get; set; }

        public Coord Destination { get; set; }
    }
}