using Assets.Scripts.Data;
using Assets.Scripts.InGame;
using Assets.Scripts.Shared.NetMessages.World;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnTeleportRequestRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnTeleport> OnTeleport;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            //1. Raise event
            var msg = (Net_OnTeleport)input;
            OnTeleport?.Invoke(msg);

            TeleportScenario scenario = ResolveScenarioType(msg);
            HeroView hero;
            if (HeroesManager.Instance.Heroes != null && HeroesManager.Instance.Heroes.Count > 0)
            {
                hero = HeroesManager.Instance.Heroes.FirstOrDefault(x => x.hero.id == msg.HeroId);
                if (hero == null || hero.isMoving)
                {
                    Debug.LogWarning("Cannot find hero to teleport!");
                    return;
                }
            }
            else
            {
                Debug.LogWarning("Heroes collection is null or empty!");
                return;
            }

            switch (scenario)
            {
                case TeleportScenario.Unknown:
                    break;
                case TeleportScenario.PlayerBlink:
                    this.HandleBlink(hero, msg);
                    break;
                case TeleportScenario.EnemyBlink:
                    this.HandleBlink(hero, msg);
                    break;
                case TeleportScenario.PlayerOut:
                    break;
                case TeleportScenario.EnemyOut:
                    break;
                case TeleportScenario.PlayerIn:
                    break;
                case TeleportScenario.EnemyIn:
                    break;
                default:
                    break;
            }

            // SCENARIOS:
            // 1. Player BLINK -> Hero region is in the currently opened region and he just change his position. Camera DO FOLLOW follow the hero.
            // 2. Enemy BLINK -> Hero region is in the currently opened region and he just change his position. Camera DO NOT FOLLOW the hero with smooth animation.
            // 3. Player TELEPORT OUT -> Hero changes regions. Unload current region and load the new one.
            // 4. Enemy TELEPORT OUT -> Hero is enemy hero so we just need to make it disapear of our map.
            // 5. Player TELEPORT IN -> See Scenario 3.
            // 6. Enemy TELEPORT IN -> Hero is enemy hero so we need to spawn it on our map!
        }

        private void HandleBlink(HeroView hero, Net_OnTeleport msg)
        {
            hero.Blink(msg.Destination);
        }

        private TeleportScenario ResolveScenarioType(Net_OnTeleport msg)
        {
            if (DataManager.Instance.ActiveRegionId == msg.RegionId && DataManager.Instance.ActiveHeroId == msg.HeroId)
            {
                return TeleportScenario.PlayerBlink;
            }

            return TeleportScenario.Unknown;
        }
    }

    public enum TeleportScenario
    {
        Unknown,
        PlayerBlink,
        EnemyBlink,
        PlayerOut,
        EnemyOut,
        PlayerIn,
        EnemyIn
    }
}