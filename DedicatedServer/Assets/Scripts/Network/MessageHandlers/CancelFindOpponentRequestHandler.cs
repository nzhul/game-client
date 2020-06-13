using Assets.Scripts.Network.Matchmaking;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class CancelFindOpponentRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_CancelFindOpponentRequest msg = (Net_CancelFindOpponentRequest)input;

            var connection = NetworkServer.Instance.Connections[connectionId];

            Matchmaker.Instance.UnRegisterPlayer(connection);
        }
    }
}