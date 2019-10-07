using Assets.Scripts.Shared.DataModels.Units;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Shared.DataModels
{
    public class Hero : Unit
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HeroType HeroType { get; set; }

        public string Name { get; set; }

        public DateTime LastActivity { get; set; }

        public TimeSpan TimePlayed { get; set; }

        public string Faction { get; set; }

        public string Class { get; set; }

        public NPCData NpcData { get; set; }

        public bool IsNPC { get; set; }

        public IList<Unit> Units { get; set; }
    }
}