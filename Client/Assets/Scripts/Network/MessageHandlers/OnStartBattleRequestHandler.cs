using Assets.Scripts.Data;
using Assets.Scripts.InGame;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World;
using System;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnStartBattleRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnStartBattle> OnStartBattle;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // 1. Raise event
            var msg = (Net_OnStartBattle)input;

            OnStartBattle?.Invoke(msg);

            // 2. Get the hero and the monster from HeroManager and MonsterManager 
            Hero hero = HeroesManager.Instance.Heroes[msg.HeroId].hero;
            MonsterPack monsterPack = MonstersManager.Instance.Monsters[msg.MonsterId].monster;

            // 3. Load them into new BattleDataManager (not destroyed on load)
            DataManager.Instance.BattleData.Hero = hero;
            DataManager.Instance.BattleData.MonsterPack = monsterPack;

            // 3. Load new scene 5_Battle 
            LevelLoader.LoadLevel(LevelLoader.BATTLE_SCENE);

            // TODO: LoadSceneAsync and show loading animation while the scene is loading.
            // Ref: https://stackoverflow.com/a/50007367
            // Brakeys tutorial -> https://www.youtube.com/watch?v=YMj2qPq9CP8
        }
    }
}
