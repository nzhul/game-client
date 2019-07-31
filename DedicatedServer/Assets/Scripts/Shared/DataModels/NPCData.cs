using System;

namespace Assets.Scripts.Shared.DataModels
{
    [Serializable]
    public class NPCData
    {
        public CreatureType mapRepresentation;
        public int x;
        public int y;
        public int disposition; // enum
        public int rewardType; // enum
        public int rewardQuantity;
        // itemReward ... TBD
        public int troopsRewardType; // enum
        public int troopsRewardQuantity;
        public string occupiedTilesString; // parse
        public string name;
        public DateTime lastDefeat;
        public bool isLocked;
    }
}
