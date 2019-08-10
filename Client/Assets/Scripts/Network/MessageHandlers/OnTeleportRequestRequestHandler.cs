using Assets.Scripts.Data;
using Assets.Scripts.InGame;
using Assets.Scripts.LevelManagement;
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
            HeroView hero = null;

            if (scenario != TeleportScenario.EnemyIn)
            {
                if (HeroesManager.Instance.Heroes != null && HeroesManager.Instance.Heroes.Count > 0)
                {
                    hero = HeroesManager.Instance.Heroes[msg.HeroId];
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
                    this.HandlePlayerOut(hero, msg);
                    break;
                case TeleportScenario.EnemyOut:
                    this.HandleEnemyOut(hero, msg);
                    break;
                case TeleportScenario.PlayerIn:
                    break;
                case TeleportScenario.EnemyIn:
                    this.HandleEnemyIn(msg);
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

        private void HandleEnemyIn(Net_OnTeleport msg)
        {
            MapManager.Instance.LoadHero(msg.HeroId, msg.Destination);
        }

        private void HandleEnemyOut(HeroView hero, Net_OnTeleport msg)
        {
            hero.TeleportOut();
        }

        private void HandlePlayerOut(HeroView hero, Net_OnTeleport msg)
        {
            // TODO: play particle effect and delay 1-2 seconds before switching scenes
            DataManager.Instance.ActiveRegionId = msg.RegionId;
            DataManager.Instance.Avatar.Heroes.FirstOrDefault(h => h.Id == hero.hero.Id).RegionId = msg.RegionId;
            DataManager.Instance.Save();
            LevelLoader.LoadLevel(LevelLoader.WORLD_SCENE);
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

            if (DataManager.Instance.ActiveRegionId != msg.RegionId && DataManager.Instance.Avatar.Heroes.Any(h => h.Id == msg.HeroId))
            {
                return TeleportScenario.PlayerOut;
            }

            if (DataManager.Instance.ActiveRegionId == msg.RegionId && !DataManager.Instance.Avatar.Heroes.Any(h => h.Id == msg.HeroId))
            {
                if (HeroesManager.Instance.Heroes.ContainsKey(msg.HeroId))
                {
                    return TeleportScenario.EnemyBlink;
                }
                else
                {
                    return TeleportScenario.EnemyIn;
                }
            }

            if (DataManager.Instance.ActiveRegionId != msg.RegionId && !DataManager.Instance.Avatar.Heroes.Any(h => h.Id == msg.HeroId))
            {
                return TeleportScenario.EnemyOut;
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