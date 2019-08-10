using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Shared.DataModels;

namespace Assets.Scripts.Shared.DataModels
{
    public class UserAvatar
    {
        public int Id { get; set; }

        public int Wood { get; set; }

        public int Ore { get; set; }

        public int Gold { get; set; }

        public int Gems { get; set; }

        public IList<Hero> Heroes { get; set; }

        public IList<Dwelling> Dwellings { get; set; }

        public IList<Waypoint> Waypoints { get; set; }
    }
}