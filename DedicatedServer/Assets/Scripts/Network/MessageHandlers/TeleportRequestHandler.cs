using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Games;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Shared.NetMessages.World.ClientServer;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.World.ServerClient;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class TeleportRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // TODO: This code is garbage:
            // Replace it with this at some point:
            // http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/
            /// https://assetstore.unity.com/packages/tools/integration/async-await-support-101056
            Net_TeleportRequest msg = (Net_TeleportRequest)input;
            InnerTRHandler innerHandler = new InnerTRHandler();
            innerHandler.Handle(connectionId, channelId, recievingHostId, msg);
        }
    }

    public class InnerTRHandler : MovementHandlerBase
    {
        public Net_OnTeleport Rmsg { get; set; }

        public int ConnectionId { get; set; }

        public Game Game { get; set; }

        public Army MovingArmy { get; set; }

        public int RecievingHostId { get; set; }

        public void Handle(int connectionId, int channelId, int recievingHostId, Net_TeleportRequest msg)
        {
            Debug.Log("Handling new Teleport request: " + JsonConvert.SerializeObject(msg, Formatting.Indented));

            this.Rmsg = new Net_OnTeleport();
            this.RecievingHostId = recievingHostId;
            this.ConnectionId = connectionId;
            this.Game = GameManager.Instance.GetGameByConnectionId(connectionId);
            this.MovingArmy = this.Game.Armies.FirstOrDefault(x => x.Id == msg.ArmyId);

            if (IsDestinationValid(msg))
            {
                Rmsg.Success = 1;
                Rmsg.ArmyId = msg.ArmyId;
                Rmsg.GameId = msg.GameId;
                Rmsg.DwellingId = msg.DwellingId;

                Reply();
            }
            else
            {
                this.Rmsg.Error = "Requested teleport destination is not valid!";
                this.Rmsg.Success = 0;
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, this.Rmsg);
            }
        }

        private void Reply()
        {
            this.Rmsg.Destination = this.CalculateDestination();

            base.UpdateCache(this.MovingArmy, this.Rmsg.Destination, this.Rmsg.GameId);
            base.NotifyClientsInGame(this.Game.Id, this.RecievingHostId, this.Rmsg);
            RequestManagerHttp.ArmiesService.UpdateArmyPosition(this.MovingArmy.Id, this.Rmsg.Destination.X, this.Rmsg.Destination.Y);
            //this.UpdateDatabase(this.ConnectionId, this.MovingArmy.Id, this.Rmsg.Destination, this.Rmsg.GameId);

            // TODO: DO the same stuff as in MapMovementRequestHandler: UpdateCache, Database, NotifyAllInterestedClients.
            // Consider extracting this common logic into class or something...
        }

        private Coord CalculateDestination()
        {
            var targetDwelling = this.Game.Dwellings.FirstOrDefault(d => d.Id == this.Rmsg.DwellingId);

            Coord freeNode = this.FindFreeNode(this.Game, new Coord { X = targetDwelling.X, Y = targetDwelling.Y });

            return freeNode;
        }

        private Coord FindFreeNode(Game game, Coord dwellingPosition)
        {
            int[,] matrix = game.Matrix;
            int searchRadius = 1;
            int searchLineLenght = 3;

            while (searchRadius <= 10)
            {
                Coord checkPosition = new Coord
                {
                    X = dwellingPosition.X + searchRadius,
                    Y = dwellingPosition.Y - searchRadius
                };

                // search bottom
                for (int i = 0; i < searchLineLenght; i++)
                {
                    // hacky -> for the first iteration i want to search in specific order -> X, X + 1 and X - 1
                    // and not X + 1, X and X - 1
                    if (i == 0 && searchRadius == 1)
                    {
                        checkPosition.X = dwellingPosition.X;
                    }

                    if (i == 1 && searchRadius == 1)
                    {
                        checkPosition.X = dwellingPosition.X + 1;
                    }

                    if (i == 3 && searchRadius == 1)
                    {
                        checkPosition.X = dwellingPosition.X - 1;
                    }

                    if (IsInsideGameBoard(checkPosition, matrix) && matrix[checkPosition.X, checkPosition.Y] == 0)
                    {
                        if (!ArmyIsOnThisPosition(game.Armies, checkPosition))
                        {
                            return checkPosition;
                        }
                    }

                    checkPosition.X--;
                }

                // search left
                for (int i = 0; i < searchLineLenght - 1; i++)
                {
                    checkPosition.Y++;
                    if (IsInsideGameBoard(checkPosition, matrix) && matrix[checkPosition.X, checkPosition.Y] == 0)
                    {
                        if (!ArmyIsOnThisPosition(game.Armies, checkPosition))
                        {
                            return checkPosition;
                        }
                    }
                }

                // search top
                for (int i = 0; i < searchLineLenght - 1; i++)
                {
                    checkPosition.X++;
                    if (IsInsideGameBoard(checkPosition, matrix) && matrix[checkPosition.X, checkPosition.Y] == 0)
                    {
                        if (!ArmyIsOnThisPosition(game.Armies, checkPosition))
                        {
                            return checkPosition;
                        }
                    }
                }

                // search right
                for (int i = 0; i < searchLineLenght - 1; i++)
                {
                    checkPosition.Y--;
                    if (IsInsideGameBoard(checkPosition, matrix) && matrix[checkPosition.X, checkPosition.Y] == 0)
                    {
                        if (!ArmyIsOnThisPosition(game.Armies, checkPosition))
                        {
                            return checkPosition;
                        }
                    }
                }

                searchRadius++;
                searchLineLenght += 2;
            }

            Debug.LogError("Cannot find Free node for teleportation!");
            return null;
        }

        private bool IsInsideGameBoard(Coord checkPosition, int[,] matrix)
        {
            return (checkPosition.X >= 0 && checkPosition.X <= matrix.GetLength(0)) &&
                        (checkPosition.Y >= 0 && checkPosition.Y <= matrix.GetLength(1));
        }

        private bool ArmyIsOnThisPosition(IList<Army> heroesInRegion, Coord checkPosition)
        {
            return heroesInRegion.Any(h => h.X == checkPosition.X && h.Y == checkPosition.Y);
        }

        private bool IsDestinationValid(Net_TeleportRequest msg)
        {
            // TODO: implement this
            return true;
        }
    }
}