using System;

namespace Assets.Scripts.Data.Models
{
    [Serializable]
    public class UserAvatar
    {
        public int id;
        public int wood;
        public int ore;
        public int gold;
        public int gems;
        public Hero[] heroes;
    }
}
