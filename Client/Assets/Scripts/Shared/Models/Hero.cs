using System.Collections.Generic;
using Assets.Scripts.Shared.Models.Units;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Assets.Scripts.Shared.Models
{
    public class Hero : Unit
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HeroType HeroType { get; set; }

        public HeroClass Class { get; set; }

        public NPCData NpcData { get; set; }

        public bool IsNPC { get; set; }

        public IList<Unit> Units { get; set; }
    }
}