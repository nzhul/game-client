using System;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;

namespace Assets.Scripts.Network.Services.TCP
{
    public class GameService : IGameService
    {
        public void Reconnect(int gameId)
        {
            var msg = new Net_ReconnectRequest()
            {
                GameId = gameId
            };

            NetworkClient.Instance.SendServer(msg);
        }

        public void ReconnectBattle(int gameId, Guid battleId)
        {
            var msg = new Net_ReconnectBattleRequest()
            {
                BattleId = battleId,
                GameId = gameId
            };

            NetworkClient.Instance.SendServer(msg);
        }
    }
}
