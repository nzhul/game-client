using System.Collections.Generic;
using Assets.Scripts.Shared.Models.Units;

namespace Assets.Scripts.Shared.Models
{
    public class Army
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int GameId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public NPCData NPCData { get; set; }

        public bool IsNPC { get; set; }

        public Team Team { get; set; }

        public IList<Unit> Units { get; set; }
    }
}
