﻿using Assets.Scripts.Data;
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
            MonsterPack attackerMonster = null;
            MonsterPack defenderMonster = null;
            int currentPlayerId = 0;
            PlayerType attackerType = PlayerType.Human;
            PlayerType defenderType = PlayerType.MonsterAI;

            if (msg.BattleScenario == BattleScenario.HUvsMonsterAI)
            {
                attackHero = HeroesManager.Instance.Heroes[msg.AttackerId].hero;
                defenderMonster = MonstersManager.Instance.Monsters[msg.DefenderId].monster;
                currentPlayerId = msg.AttackerId;
                attackerType = PlayerType.Human;
                defenderType = PlayerType.MonsterAI;
            }
            else if (msg.BattleScenario == BattleScenario.HUAIvsMonsterAI)
            {
                attackHero = HeroesManager.Instance.Heroes[msg.AttackerId].hero;
                MonsterPack monsterPack = MonstersManager.Instance.Monsters[msg.DefenderId].monster;
                currentPlayerId = msg.AttackerId;
                attackerType = PlayerType.HumanAI;
                defenderType = PlayerType.MonsterAI;
            }
            else if (msg.AttackerType == PlayerType.Human && msg.DefenderType == PlayerType.Human)
            {
                attackHero = HeroesManager.Instance.Heroes[msg.AttackerId].hero;
                defenderHero = HeroesManager.Instance.Heroes[msg.DefenderId].hero;
                currentPlayerId = msg.AttackerId;
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
            bd.AttackerMonster = attackerMonster;
            bd.DefenderMonster = defenderMonster;
            bd.AttackerId = msg.AttackerId;
            bd.DefenderId = msg.DefenderId;
            bd.CurrentPlayerId = currentPlayerId;
            bd.BattleId = msg.BattleId;
            bd.Turn = Turn.Attacker;
            bd.RemainingTimeForThisTurn = BattleManager.TURN_DURATION;
            bd.ActionsEnabled = true;

            return bd;
        }
    }
}