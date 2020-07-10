using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
using UnityEngine;

namespace Assets.Scripts.Games
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private IDictionary<int, Game> Games;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }

            this.Games = new Dictionary<int, Game>();
        }

        public bool GameIsRegistered(int gameId)
        {
            return this.Games.ContainsKey(gameId);
        }

        public void RegisterGame(Game game)
        {
            this.Games.Add(game.Id, game);
        }

        public Game GetGameByConnectionId(int connectionId)
        {
            var gameId = this.GetGameIdByConnectionId(connectionId);
            if (gameId == 0)
            {
                Debug.LogWarning($"No active game was found for connectionId: {connectionId}");
            }

            return this.Games[gameId];
        }

        public Unit GetUnit(int gameId, int unitId)
        {
            var game = this.Games[gameId];

            foreach (var army in game.Armies)
            {
                var unit = army.Units.FirstOrDefault(x => x.Id == unitId);
                if (unit != null)
                {
                    return unit;
                }
            }

            return null;
        }

        public Army GetArmy(int gameId, int armyId)
        {
            var game = this.Games[gameId];
            return game.Armies.FirstOrDefault(x => x.Id == armyId);
        }

        public int GetGameIdByConnectionId(int connectionId)
        {
            return NetworkServer.Instance.Connections[connectionId].GameId;
        }

        public int GetConnectionIdByArmyId(int gameId, int armyId)
        {
            var army = this.Games[gameId].Armies.FirstOrDefault(x => x.Id == armyId);
            var connection = NetworkServer.Instance.Connections.FirstOrDefault(x => x.Value.UserId == army.UserId);
            return connection.Value != null ? connection.Value.ConnectionId : 0;
        }
    }

}