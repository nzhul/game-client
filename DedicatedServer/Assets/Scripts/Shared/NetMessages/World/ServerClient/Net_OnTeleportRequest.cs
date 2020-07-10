﻿using System;
using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Shared.NetMessages.World.ServerClient
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

        public int ArmyId { get; set; }

        public Coord Destination { get; set; }

        public int GameId { get; set; }

        public int DwellingId { get; set; }
    }
}