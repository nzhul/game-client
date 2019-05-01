using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Shared.DataModels;

namespace Assets.Scripts.Shared.DataModels
{
    [Serializable]
    public class UserAvatar
    {
        public int id;
        public int wood;
        public int ore;
        public int gold;
        public int gems;
        public IList<Hero> heroes;
        public IList<Dwelling> dwellings;
        public IList<Waypoint> waypoints;
    }
}