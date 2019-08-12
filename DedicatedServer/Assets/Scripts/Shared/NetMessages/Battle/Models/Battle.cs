using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
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

        public int AttackerConnectionId { get; set; }

        public Hero AttackerHero { get; set; }

        public int DefenderId { get; set; }

        public int DefenderConnectionId { get; set; }

        public Hero DefenderHero { get; set; }

        public int CurrentPlayerId { get; set; }

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
