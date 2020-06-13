using Assets.Scripts.Network.Services.HTTP.Interfaces;
using Assets.Scripts.Shared.Models;

namespace Assets.Scripts.Network.Services.HTTP
{
    public class GameService : BaseService, IGameService
    {
        public Game CreateGame(GameParams gameConfig)
        {
            return base.Post<Game>($"games", gameConfig);
        }
    }
}
