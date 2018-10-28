using System.Collections.Generic;

namespace Assets.Scripts.Data.Models
{
    public class Region
    {
        public int id;
        public string name;
        public int level;
        public int realmId;
        public string matrixString;
        public IList<Room> rooms;
        public IList<Hero> heroes;
        public IList<MonsterPack> monsterPacks;
        public IList<Dwelling> dwellings;
    }
}
