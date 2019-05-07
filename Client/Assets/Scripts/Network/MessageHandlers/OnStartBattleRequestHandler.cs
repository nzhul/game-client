using Assets.Scripts.Shared.NetMessages.World;
using System;
using UnityEngine;

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
            // 3. Load them into new BattleDataManager (not destroyed on load)
            // 3. Load new scene 5_Battle 

            Debug.Log($"Starting battle with hero {msg.HeroId} and monster {msg.MonsterId}");
        }
    }
}
