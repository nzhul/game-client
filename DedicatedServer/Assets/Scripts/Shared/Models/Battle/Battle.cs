using System;
using System.Collections.Generic;
using Assets.Scripts.Shared.Models.Units;

namespace Assets.Scripts.Shared.Models
{
    public class Battle
    {
        public Battle()
        {
            this.Start = DateTime.UtcNow;
            this.AttackerLastActivity = DateTime.UtcNow;
            this.DefenderLastActivity = DateTime.UtcNow;
            this.Log = new List<string>();
        }

        public Guid Id { get; set; }

        public int AttackerId { get; set; }

        public int AttackerConnectionId { get; set; }

        public Hero AttackerHero { get; set; }

        public DateTime AttackerLastActivity { get; set; }

        public int DefenderId { get; set; }

        public int DefenderConnectionId { get; set; }

        public Hero DefenderHero { get; set; }

        public DateTime DefenderLastActivity { get; set; }

        public int CurrentHeroId { get; set; }

        public Unit SelectedUnit { get; set; }

        public PlayerType AttackerType { get; set; }

        public PlayerType DefenderType { get; set; }

        public bool AttackerReady { get; set; }

        public bool DefenderReady { get; set; }

        public BattleState State { get; set; }

        public BattleScenario BattleScenario { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public float LastTurnStartTime { get; set; }

        public Turn Turn { get; set; }

        public List<string> Log { get; set; }

        public void UpdateLastActivity(int heroId)
        {
            if (this.AttackerId == heroId)
            {
                this.AttackerLastActivity = DateTime.UtcNow;
            }
            else if (this.DefenderId == heroId)
            {
                this.DefenderLastActivity = DateTime.UtcNow;
            }
        }

        // Attacker Troops

        // Defender Troops

        // BATTLE FLOW
        // 1. Battle is registered in server scheduler
        // 2. Battle is in pause state on both clients
        // 3. Scheduler sends SwapTurn message to the clients by giving the first turn to the attacker
        // 4. Scheduler track turn start time and compares whether it is > than 75 seconds every 5 seconds.
        // 5. If true -> sends another swap message
    }
}
