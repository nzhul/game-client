namespace Assets.Scripts.Shared.DataModels
{
    public class MonsterPack
    {
        public int id;
        public int type; // enum
        public int x;
        public int y;
        public int quantity;
        public int disposition; // enum
        public int rewardType; // enum
        public int rewardQuantity;
        // itemReward ... TBD
        public int troopsRewardType; // enum
        public int troopsRewardQuantity;
        public string occupiedTilesString; // parse
        public string name;
    }
}
