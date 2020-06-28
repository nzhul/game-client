using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Games;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.Models;
using Assets.Utilities;
using UnityEngine;

namespace Assets.Scripts.Network.Matchmaking
{
    public class Matchmaker : MonoBehaviour
    {
        private static Matchmaker _instance;

        public static Matchmaker Instance
        {
            get
            {
                return _instance;
            }
        }

        private List<MMRequest> Pool;

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

            this.Pool = new List<MMRequest>();
        }

        public void RegisterPlayer(ServerConnection connection, CreatureType @class)
        {
            var request = new MMRequest()
            {
                Connection = connection,
                SearchStart = DateTime.UtcNow,
                StartingClass = @class
            };

            this.Pool.Add(request);
            Debug.Log($"{request.Connection.Username} registered in matchmaking pool " +
                $"with {request.Connection.MMR} MMR and {@class} class.");
        }

        public void UnRegisterPlayer(ServerConnection connection)
        {
            var request = this.Pool.FirstOrDefault(x => x.Connection.ConnectionId == connection.ConnectionId);

            if (request == null)
            {
                return;
            }

            this.Pool.Remove(request);
            Debug.Log($"{request.Connection.Username} canceled his MM request.");
        }

        public void DoMatchmaking()
        {
            var matchedRequests = new List<MMRequest>();
            foreach (var request in this.Pool)
            {
                var match = this.Pool.FirstOrDefault(
                    x => !x.MatchFound &&
                    x.SearchRange.Overlap(request.SearchRange)
                    && x.Connection.ConnectionId != request.Connection.ConnectionId);

                if (match != null)
                {
                    Debug.Log($"Match found for players: {request.Connection.Username} and {match.Connection.Username}");

                    request.MatchFound = true;
                    match.MatchFound = true;
                    matchedRequests.Add(request);
                    matchedRequests.Add(match);

                    var @params = new GameParams
                    {
                        Players = new List<Player>
                        {
                            new Player
                            {
                                UserId = request.Connection.UserId,
                                Team = Team.Team1,
                                StartingClass = request.StartingClass
                            },
                            new Player
                            {
                                UserId = match.Connection.UserId,
                                Team = Team.Team2,
                                StartingClass = match.StartingClass
                            }
                        }
                    };

                    var game = RequestManagerHttp.GameService.CreateGame(@params);
                    GameManager.Instance.RegisterGame(game);

                    var connectionIds = new int[] { request.Connection.ConnectionId, match.Connection.ConnectionId };
                    RequestManagerTcp.GameService.StartGame(game.Id, connectionIds);
                }
            }

            foreach (var request in matchedRequests)
            {
                this.Pool.Remove(request);
            }
        }
    }
}
