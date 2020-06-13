using System.Linq;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;

namespace Assets.Scripts.Network.MessageHandlers
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

                var movingHero = NetworkServer.Instance.Connections[connectionId]?.Avatar?.Heroes?.FirstOrDefault(h => h.Id == msg.HeroId);

                // 2. Notify the interested clients ( must exclude the requester )
                base.NotifyClientsInRegion(movingHero.GameId, recievingHostId, rmsg);

                // 3. Update hero position here in the dedicated server cache.
                base.UpdateCache(movingHero, msg.Destination, movingHero.GameId);

                // 5. Update the database.
                base.UpdateDatabase(connectionId, msg.HeroId, msg.Destination, movingHero.GameId);

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
