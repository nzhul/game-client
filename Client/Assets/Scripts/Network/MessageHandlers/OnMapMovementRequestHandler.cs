using Assets.Scripts.InGame;
using Assets.Scripts.Shared.NetMessages.World;
using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnMapMovementRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnMapMovement> OnMapMovement;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            //1. Raise event
            var msg = (Net_OnMapMovement)input;
            OnMapMovement?.Invoke(msg);

            if (HeroesManager.Instance.Heroes != null && HeroesManager.Instance.Heroes.Count > 0)
            {
                var hero = HeroesManager.Instance.Heroes.FirstOrDefault(x => x.hero.id == msg.HeroId);
                if (hero != null && !hero.isMoving)
                {
                    hero.MoveToNode(msg.Destination);
                }
                else
                {
                    Debug.LogWarning("Cannot find hero to move!");
                }
            }
            else
            {
                Debug.LogWarning("Heroes collection is null or empty!");
            }
        }
    }
}