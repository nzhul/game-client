using System;
using Assets.Scripts.Data;
using Assets.Scripts.InGame;
using Assets.Scripts.InGame.Map.Entities;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;
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
            AliveEntityView entity = null;


            if (AliveEntitiesManager.Instance.Entities != null && AliveEntitiesManager.Instance.Entities.Count > 0)
            {
                entity = AliveEntitiesManager.Instance.Entities[msg.ArmyId];
                if (entity == null || entity.isMoving)
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
                    this.HandleBlink(entity, msg);
                    break;
                case TeleportScenario.EnemyBlink:
                    this.HandleBlink(entity, msg);
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

        //private void HandleEnemyIn(Net_OnTeleport msg)
        //{
        //    MapManager.Instance.LoadHero(msg.HeroId, msg.Destination);
        //}

        //private void HandleEnemyOut(ArmyView hero, Net_OnTeleport msg)
        //{
        //    hero.TeleportOut();
        //}

        //private void HandlePlayerOut(ArmyView hero, Net_OnTeleport msg)
        //{
        //    // TODO: play particle effect and delay 1-2 seconds before switching scenes
        //    DataManager.Instance.ActiveGameId = msg.GameId;
        //    //DataManager.Instance.Avatar.Heroes.FirstOrDefault(h => h.Id == hero.rawUnit.Id).GameId = msg.RegionId;
        //    DataManager.Instance.Save();
        //    LevelLoader.LoadLevel(LevelLoader.GAME_SCENE);
        //}

        private void HandleBlink(AliveEntityView entity, Net_OnTeleport msg)
        {
            entity.Blink(msg.Destination);
        }

        private TeleportScenario ResolveScenarioType(Net_OnTeleport msg)
        {
            var teleportingArmy = DataManager.Instance.ActiveGame.GetArmy(msg.ArmyId);

            if (teleportingArmy.Team == DataManager.Instance.Avatar.Team)
            {
                return TeleportScenario.PlayerBlink;
            }
            else
            {
                return TeleportScenario.EnemyBlink;
            }
        }
    }

    public enum TeleportScenario
    {
        Unknown,
        PlayerBlink,
        EnemyBlink
    }
}