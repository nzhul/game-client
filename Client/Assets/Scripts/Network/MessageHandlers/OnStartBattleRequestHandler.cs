using System;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnStartBattleRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnStartBattle> OnStartBattle;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // 1. Parse the input
            var msg = (Net_OnStartBattle)input;

            // 2. Load them into new BattleDataManager (not destroyed on load)
            DataManager.Instance.BattleData = this.ResolveBattleData(msg);

            // 3. Raise event
            OnStartBattle?.Invoke(msg);

            // 4. Load new scene 5_Battle 
            LevelLoader.LoadLevel(LevelLoader.BATTLE_SCENE);

            // TODO: LoadSceneAsync and show loading animation while the scene is loading.
            // Ref: https://stackoverflow.com/a/50007367
            // Brakeys tutorial -> https://www.youtube.com/watch?v=YMj2qPq9CP8
        }

        private BattleData ResolveBattleData(Net_OnStartBattle msg)
        {
            Army attackArmy = null;
            Army defenderArmy = null;
            int currentArmyId = 0;
            PlayerType attackerType = PlayerType.Human;
            PlayerType defenderType = PlayerType.AI;
            PlayerType currentPlayerType = PlayerType.Human;
            attackArmy = DataManager.Instance.ActiveGame.GetArmy(msg.AttackerArmyId);
            defenderArmy = DataManager.Instance.ActiveGame.GetArmy(msg.DefenderArmyId);
            currentArmyId = msg.AttackerArmyId;

            if (msg.BattleScenario == BattleScenario.HUvsAI)
            {
                currentPlayerType = PlayerType.Human;
                attackerType = PlayerType.Human;
                defenderType = PlayerType.AI;
            }
            else if (msg.BattleScenario == BattleScenario.AIvsAI)
            {
                currentPlayerType = PlayerType.AI;
                attackerType = PlayerType.AI;
                defenderType = PlayerType.AI;
            }
            else if (msg.BattleScenario == BattleScenario.HUvsHU)
            {
                currentPlayerType = PlayerType.Human;
                attackerType = PlayerType.Human;
                defenderType = PlayerType.Human;
            }
            else if (false)
            {
                // TODO:
                // MonsterAIvsHU,
                // MonsterAIvsHUAI
            }

            var bd = new BattleData();
            bd.BattleScenario = msg.BattleScenario;
            bd.AttackerType = attackerType;
            bd.DefenderType = defenderType;
            bd.AttackerArmy = attackArmy;
            bd.DefenderArmy = defenderArmy;
            bd.AttackerArmyId = msg.AttackerArmyId;
            bd.DefenderArmyId = msg.DefenderArmyId;
            bd.CurrentArmyId = currentArmyId;
            bd.SelectedUnit = DataManager.Instance.ActiveGame.GetUnit(msg.SelectedUnitId);
            bd.BattleId = msg.BattleId;
            bd.Turn = Turn.Attacker;
            bd.RemainingTimeForThisTurn = BattleManager.TURN_DURATION;
            bd.ActionsEnabled = true;
            this.UpdateUnitsData(bd.AttackerArmy);
            this.UpdateUnitsData(bd.DefenderArmy);

            return bd;
        }

        private void UpdateUnitsData(Army hero)
        {
            //TODO apply upgrades before the battle!
            foreach (var unit in hero.Units)
            {
                var config = DataManager.Instance.UnitConfigurations[unit.Type];
                unit.MovementPoints = config.MovementPoints;
                unit.MaxMovementPoints = unit.MovementPoints;
                unit.ActionPoints = config.ActionPoints;
                unit.MaxMovementPoints = unit.ActionPoints;
                unit.MinDamage = config.MinDamage;
                unit.MaxDamage = config.MaxDamage;
                unit.Hitpoints = config.Hitpoints;
                unit.BaseHitpoints = unit.Hitpoints;
                unit.MaxHitpoints = unit.Hitpoints;
                unit.Mana = config.Mana;
                unit.Armor = config.Armor;
                unit.AttackType = config.AttackType;
                unit.ArmorType = config.ArmorType;
                unit.Level = config.CreatureLevel;
            }
        }
    }
}
