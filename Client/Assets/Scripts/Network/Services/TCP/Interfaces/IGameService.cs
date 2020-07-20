using System;

namespace Assets.Scripts.Network.Services.TCP.Interfaces
{
    public interface IGameService
    {
        void Reconnect(int gameId);

        void ReconnectBattle(int gameId, Guid battleId);
    }
}
