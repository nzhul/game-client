using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public IList<MonsterPack> monsterPacks;
        public IList<Dwelling> dwellings;
    }
}
