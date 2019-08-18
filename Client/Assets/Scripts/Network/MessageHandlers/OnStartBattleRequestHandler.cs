using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.InGame;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System;

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
            Hero attackHero = null;
            Hero defenderHero = null;
            int currentHeroId = 0;
            PlayerType attackerType = PlayerType.Human;
            PlayerType defenderType = PlayerType.AI;
            PlayerType currentPlayerType = PlayerType.Human;

            if (msg.BattleScenario == BattleScenario.HUvsAI)
            {
                attackHero = HeroesManager.Instance.Heroes[msg.AttackerId].hero;
                defenderHero = HeroesManager.Instance.NPCs[msg.DefenderId].npc;
                currentHeroId = msg.AttackerId;
                currentPlayerType = PlayerType.Human;
                attackerType = PlayerType.Human;
                defenderType = PlayerType.AI;
            }
            else if (msg.BattleScenario == BattleScenario.AIvsAI)
            {
                attackHero = HeroesManager.Instance.Heroes[msg.AttackerId].hero;
                defenderHero = HeroesManager.Instance.NPCs[msg.DefenderId].npc;
                currentHeroId = msg.AttackerId;
                currentPlayerType = PlayerType.AI;
                attackerType = PlayerType.AI;
                defenderType = PlayerType.AI;
            }
            else if (msg.BattleScenario == BattleScenario.HUvsHU)
            {
                attackHero = HeroesManager.Instance.Heroes[msg.AttackerId].hero;
                defenderHero = HeroesManager.Instance.Heroes[msg.DefenderId].hero;
                currentHeroId = msg.AttackerId;
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
            bd.AttackerHero = attackHero;
            bd.DefenderHero = defenderHero;
            bd.AttackerId = msg.AttackerId;
            bd.DefenderId = msg.DefenderId;
            bd.CurrentHeroId = currentHeroId;
            bd.SelectedUnit = HeroesManager.Instance.GetUnitById(currentHeroId, msg.SelectedUnitId, currentPlayerType);
            bd.BattleId = msg.BattleId;
            bd.Turn = Turn.Attacker;
            bd.RemainingTimeForThisTurn = BattleManager.TURN_DURATION;
            bd.ActionsEnabled = true;
            this.UpdateUnitsData(bd.AttackerHero);
            this.UpdateUnitsData(bd.DefenderHero);

            return bd;
        }

        private void UpdateUnitsData(Hero hero)
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
                unit.CreatureLevel = config.CreatureLevel;
            }
        }
    }
}
