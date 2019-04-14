using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
using System.Linq;

namespace Assets.Scripts.MessageHandlers
{
    public class MapMovementRequestHandler : MovementHandlerBase, IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_MapMovementRequest msg = (Net_MapMovementRequest)input;
            Net_OnMapMovement rmsg = new Net_OnMapMovement();


            // 1. Validate the new position
            if (IsNewPositionValid(msg))
            {
                rmsg.Success = 1;
                rmsg.HeroId = msg.HeroId;
                rmsg.Destination = new Coord
                {
                    X = msg.Destination.X,
                    Y = msg.Destination.Y
                };

                var movingHero = NetworkServer.Instance.Connections[connectionId]?.Avatar?.heroes?.FirstOrDefault(h => h.id == msg.HeroId);

                // 2. Notify the interested clients ( must exclude the requester )
                base.NotifyClientsInRegion(movingHero.regionId, recievingHostId, rmsg);

                // 3. Update hero position here in the dedicated server cache.
                base.UpdateCache(movingHero, msg.Destination, movingHero.regionId);

                // 5. Update the database.
                base.UpdateDatabase(connectionId, msg.HeroId, msg.Destination, movingHero.regionId);

                // Note: Both UpdateCached and UpdateDatabase is happening after client notification.
                // That is done on purpose so we do not slow down the response to the client after we know that the request is valid.
            }
            else
            {
                rmsg.Error = "Requested position is not valid!";
                rmsg.Success = 0;
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
            }
        }

        private bool IsNewPositionValid(Net_MapMovementRequest msg)
        {
            // TODO: Implement the validation
            return true;
        }
    }
}
