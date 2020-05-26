using System;

namespace Assets.Scripts.MessageHandlers
{
    public class FindOpponentRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // Get player player mmr from API
            // register the player in NetworkServer.Instance.MatchmakingPool
        }
    }
}
