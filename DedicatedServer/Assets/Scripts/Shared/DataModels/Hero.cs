using System;
using System.Collections.Generic;

namespace Assets.Scripts.Shared.DataModels
{
    [Serializable]
    public class Hero
    {
        public int id;
        public int x;
        public int y;
        public HeroType type;
        public string name;
        public DateTime lastActivity;
        public TimeSpan timePlayed;
        public int level;
        public int attack;
        public int defence;
        public int magic;
        public int magicPower;
        public int personalAttack;
        public int personalDefense;
        public int dodge;
        public int health;
        public int minDamage;
        public int maxDamage;
        public int magicResistance;
        public string faction;
        public string @class;
        public int regionId;
        public NPCData npcData;
        public bool isNPC;
        public IList<Unit> units;
    }
}
