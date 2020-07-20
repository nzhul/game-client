using Assets.Scripts.Games;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class ReconnectRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_ReconnectRequest msg = (Net_ReconnectRequest)input;

            // TODO: User should not be able to just reconnect to any Game.
            // Server should do API call to check what is the gameId of the user.

            var connection = NetworkServer.Instance.Connections[connectionId];
            connection.GameId = msg.GameId;

            if (GameManager.Instance.GameIsRegistered(msg.GameId))
            {
                Debug.Log($"Game with Id {msg.GameId} is already registered!");
                return;
            }

            var game = RequestManagerHttp.GameService.GetGame(msg.GameId);
            GameManager.Instance.RegisterGame(game);
            Debug.Log($"Game with Id {game.Id} loaded!");
        }
    }
}
