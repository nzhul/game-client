using Assets.Scripts.Network.Matchmaking;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class FindOpponentRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // Get player player mmr from API
            // register the player in NetworkServer.Instance.MatchmakingPool

            Net_FindOpponentRequest msg = (Net_FindOpponentRequest)input;

            if (!msg.IsValid())
            {
                Debug.LogWarning($"Invalid {nameof(msg)}");
                return;
            }

            // Get player latest mmr from API
            var connection = NetworkServer.Instance.Connections[connectionId];
            var userData = RequestManagerHttp.UsersService.GetUser(connection.UserId);
            connection.MMR = userData.mmr;

            Matchmaker.Instance.RegisterPlayer(connection, msg.Class);
        }
    }
}
