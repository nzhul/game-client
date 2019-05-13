using Assets.Scripts.Shared.DataModels;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data.Models
{
    public class BattleData
    {
        public bool UseAutoBattles { get; set; }

        public Hero Hero { get; set; }

        public MonsterPack MonsterPack { get; set; }

        public int AttackerId { get; set; }

        public int DefenderId { get; set; }

        public bool AttackerIsAI { get; set; }

        public bool DefenderIsAI { get; set; }

        public DateTime Start { get; set; }

        public List<string> Log { get; set; }

        public Turn Turn { get; set; }
    }

    public enum Turn
    {
        Attacker,
        Defender
    }
}
