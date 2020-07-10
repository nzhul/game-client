using System;
using Assets.Scripts.InGame;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;
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

            //Debug.Log("Map movement request recieved: " + JsonConvert.SerializeObject(msg));

            if (AliveEntitiesManager.Instance.Entities != null && AliveEntitiesManager.Instance.Entities.Count > 0)
            {
                var army = AliveEntitiesManager.Instance.Entities[msg.ArmyId];
                if (army != null && !army.isMoving)
                {
                    army.MoveToNode(msg.Destination);
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