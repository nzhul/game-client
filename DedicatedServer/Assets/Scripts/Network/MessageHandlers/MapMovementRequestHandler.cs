using System.Linq;
using Assets.Scripts.Games;
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
                rmsg.ArmyId = msg.ArmyId;
                rmsg.Destination = new Coord
                {
                    X = msg.Destination.X,
                    Y = msg.Destination.Y
                };

                var gameId = GameManager.Instance.GetGameIdByConnectionId(connectionId);
                var movingArmy = GameManager.Instance.GetArmy(gameId, msg.ArmyId);

                // 2. Notify the interested clients ( must exclude the requester )
                base.NotifyClientsInGame(gameId, recievingHostId, rmsg);

                // 3. Update army position here in the dedicated server cache.
                base.UpdateCache(movingArmy, msg.Destination, movingArmy.GameId);

                // 5. Update the database.
                base.UpdateDatabase(connectionId, msg.ArmyId, msg.Destination, movingArmy.GameId);

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
