using System;

namespace Assets.Scripts.Shared.DataModels
{
    [Serializable]
    public class NPCData
    {
        public int id;
        public CreatureType mapRepresentation;
        public int x;
        public int y;
        public Disposition disposition; // enum
        public TreasureType rewardType; // enum
        public int rewardQuantity;
        // itemReward ... TBD
        public CreatureType troopsRewardType; // enum
        public int troopsRewardQuantity;
        public string occupiedTilesString; // parse
        public string name;
        public DateTime lastDefeat;
        public bool isLocked;
    }
}
