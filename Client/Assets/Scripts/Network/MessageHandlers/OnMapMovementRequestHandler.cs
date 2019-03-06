using System;
using Assets.Scripts.Shared.NetMessages.World;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnMapMovementRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnMapMovement> OnMapMovement;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            //1. Raise event
            var msg = (Net_OnMapMovement)input;
            if (OnMapMovement != null)
            {
                OnMapMovement(msg);
            }

            // I need to create HeroesManager class that will store a collection of all heroes on the map
            // Then i will be able to invoke HeroesManager.Instance.Heroes[heroId].MoveToNode(x, y)
            // .MoveToNode(x, y) will check if the current hero have cached path.
            // if cached path exist -> execute the path and clear the cache
            // if not -> find path, execute and clear.
        }
    }
}