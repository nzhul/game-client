using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.Battle.Models;
using Assets.Scripts.Shared.NetMessages.World.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Data.Models
{
    // TODO: Move this class in the SHARED folder
    public class BattleData
    {
        public BattleData()
        {
            this.State = BattleState.Preparation;
            this.Start = DateTime.UtcNow;
            this.Log = new List<string>();
        }

        public bool UseAutoBattles { get; set; }

        public Hero AttackerHero { get; set; }

        public Hero DefenderHero { get; set; }

        public Guid BattleId { get; set; }

        public int AttackerId { get; set; }

        public int DefenderId { get; set; }

        public int CurrentPlayerId { get; set; }

        public PlayerType AttackerType { get; set; }

        public PlayerType DefenderType { get; set; }

        public BattleState State { get; set; }

        public BattleScenario BattleScenario { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public float LastTurnStartTime { get; set; }

        public int RemainingTimeForThisTurn { get; set; }

        bool _actionsEnabled = false;

        public Turn Turn { get; set; }

        public List<string> Log { get; set; }

        [JsonIgnore]
        public bool ActionsEnabled
        {
            get
            {
                return _actionsEnabled;
            }
            set
            {
                _actionsEnabled = value;
                OnActionsEnabledChange?.Invoke(value);
            }
        }

        public static event Action<bool> OnActionsEnabledChange;
    }
}
