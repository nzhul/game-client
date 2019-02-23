using System.Collections.Generic;
using Assets.Scripts.Shared.NetMessages.World;

namespace Assets.Scripts.MessageHandlers
{
    public class MapMovementRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_MapMovementRequest msg = (Net_MapMovementRequest)input;
            Net_OnMapMovement rmsg = new Net_OnMapMovement();

            // TODO: Validate the new positions!

            if (IsNewPositionValid(msg))
            {
                rmsg.Success = 1;
                rmsg.HeroUpdates = new List<HeroUpdate>
                {
                    new HeroUpdate
                    {
                        HeroId = msg.HeroId,
                        NewX = msg.NewX,
                        NewY = msg.NewY
                    }
                };

                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);

                NotifyAllInterestedClients();
            }
            else
            {
                rmsg.Error = "Requested position is not valid!";
                rmsg.Success = 0;
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
            }
        }

        private void NotifyAllInterestedClients()
        {
            // 1. Get a list of all ServerConnections that have a region that contain our hero
            //    var connectionsToNotify = ...
            // 2. foreach connection -> SendClient(recievingHostId, clientConnectionId, rmsg);
        }

        private bool IsNewPositionValid(Net_MapMovementRequest msg)
        {
            // TODO: Implement the validation
            return true;
        }
    }
}
