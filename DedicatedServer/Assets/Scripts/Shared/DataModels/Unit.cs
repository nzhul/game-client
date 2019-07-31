﻿using System;

namespace Assets.Scripts.Shared.DataModels
{
    [Serializable]
    public class Unit
    {
        public int id;
        public int x;
        public int y;
        public CreatureType type;
        public int quantity;

        #region Non-Persistend properties

        // The non-persistend properties will not be stored in database and won't be unique per unit
        // Instead there will be special UnitConfigurations table in the database where we will store configuration for each type of unit
        // For example:
        // Bowman:
        // BaseMovePoints: 2
        // BaseActionPoints: 1
        // MinDamage: 10
        // MaxDamage: 15
        // ...
        // Those properties will be loaded on server start.

        public int MovementPoints { get; set; }

        public int MaxMovementPoints { get; set; }

        public int ActionPoints { get; set; }

        public int MaxActionPoints { get; set; }

        public int MinDamage { get; set; }

        public int MaxDamage { get; set; }

        /// <summary>
        /// Current hitpoints of the creature.
        /// </summary>
        public int Hitpoints { get; set; }

        /// <summary>
        /// Hitpoints of the creature at the start of the battle -> this.Config.BaseHitpoints + Upgrades
        /// </summary>
        public int BaseHitpoints { get; set; }

        /// <summary>
        /// Current Max hitpoints of the creature -> this.BaseHitpoints + Buffs/Debufs
        /// </summary>
        public int MaxHitpoints { get; set; }

        #endregion
    }
}
