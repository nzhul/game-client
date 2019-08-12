﻿using Assets.Scripts.Shared.DataModels.Units;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Shared.DataModels
{
    public class Hero
    {
        public int Id { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public HeroType Type { get; set; }

        public string Name { get; set; }

        public DateTime LastActivity { get; set; }

        public TimeSpan TimePlayed { get; set; }

        public int Level { get; set; }

        public int Attack { get; set; }

        public int Defence { get; set; }

        public int Magic { get; set; }

        public int MagicPower { get; set; }

        public int PersonalAttack { get; set; }

        public int PersonalDefense { get; set; }

        public int Dodge { get; set; }

        public int Health { get; set; }

        public int MinDamage { get; set; }

        public int MaxDamage { get; set; }

        public int MagicResistance { get; set; }

        public string Faction { get; set; }

        public string Class { get; set; }

        public int RegionId { get; set; }

        public NPCData NpcData { get; set; }

        public bool IsNPC { get; set; }

        public IList<Unit> Units { get; set; }
    }
}