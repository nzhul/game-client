using System.Collections.Generic;
using Assets.Scripts.Shared.Models.Units;

namespace Assets.Scripts.Shared.Models
{
    public class Army : GridEntity
    {
        public int? UserId { get; set; }

        public string Username { get; set; }

        public int GameId { get; set; }

        public NPCData NPCData { get; set; }

        public bool IsNPC { get; set; }

        public Team Team { get; set; }

        public IList<Unit> Units { get; set; }
    }
}
