﻿using System;
using System.Collections.Generic;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using Newtonsoft.Json;

namespace Assets.Scripts.Data.Models
{
    // TODO: Move this class in the SHARED folder
    public class BattleData
    {
        public BattleData(bool playerIsAttacker = false)
        {
            this.PlayerIsAttacker = playerIsAttacker;
            this.State = BattleState.Preparation;
            this.Start = DateTime.UtcNow;
            this.Log = new List<string>();
        }

        public bool UseAutoBattles { get; set; }

        public Army PlayerArmy { get; set; }

        public Army EnemyArmy { get; set; }

        public int BoardRowCount { get; set; } = 11;

        public int BoardColumnCount { get; set; } = 15;

        public Guid BattleId { get; set; }

        public int PlayerArmyId { get; set; }

        public int EnemyArmyId { get; set; }

        public Unit SelectedUnit { get; set; }

        public int CurrentArmyId { get; set; }

        public PlayerType CurrentPlayerType { get; set; }

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

        public bool PlayerIsAttacker { get; }

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
