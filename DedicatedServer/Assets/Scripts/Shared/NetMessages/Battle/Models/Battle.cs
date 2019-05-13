using System;
using System.Collections.Generic;

namespace Assets.Scripts.Shared.NetMessages.Battle.Models
{
    public class Battle
    {
        public Battle()
        {
            this.Start = DateTime.UtcNow;
            this.Log = new List<string>();
        }

        public Guid Id { get; set; }

        public int AttackerId { get; set; }

        public int DefenderId { get; set; }

        public bool AttackerIsAI { get; set; }

        public bool DefenderIsAI { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public List<string> Log { get; set; }

        // Attacker Troops

        // Defender Troops
    }
}
