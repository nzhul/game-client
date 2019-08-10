using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Network.Shared.NetMessages.World;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World;
using Assets.Scripts.Shared.NetMessages.World.Models;
using BestHTTP;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
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

        public Hero MovingHero { get; set; }

        public int RecievingHostId { get; set; }

        public void Handle(int connectionId, int channelId, int recievingHostId, Net_TeleportRequest msg)
        {
            Debug.Log("Handling new Teleport request: " + JsonConvert.SerializeObject(msg, Formatting.Indented));

            this.Rmsg = new Net_OnTeleport();
            this.RecievingHostId = recievingHostId;
            this.ConnectionId = connectionId;
            this.MovingHero = NetworkServer.Instance.Connections[connectionId]?.Avatar?.Heroes?.FirstOrDefault(h => h.Id == msg.HeroId);

            if (IsDestinationValid(msg))
            {
                Rmsg.Success = 1;
                Rmsg.HeroId = msg.HeroId;
                Rmsg.RegionId = msg.RegionId;
                Rmsg.DwellingId = msg.DwellingId;

                if (!NetworkServer.Instance.Regions.ContainsKey(msg.RegionId))
                {
                    LoadAndReply();
                }
                else
                {
                    Reply();
                }
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
            //NetworkServer.Instance.SendClient(this.RecievingHostId, this.ConnectionId, this.Rmsg);

            int oldRegionId = this.MovingHero.RegionId;
            int newRegionId = this.Rmsg.RegionId;

            base.UpdateCache(this.MovingHero, this.Rmsg.Destination, this.Rmsg.RegionId);
            base.NotifyClientsInRegion(newRegionId, this.RecievingHostId, this.Rmsg);
            base.NotifyClientsInRegion(oldRegionId, this.RecievingHostId, this.Rmsg);
            this.UpdateDatabase(this.ConnectionId, this.MovingHero.Id, this.Rmsg.Destination, this.Rmsg.RegionId);

            // TODO: DO the same stuff as in MapMovementRequestHandler: UpdateCache, Database, NotifyAllInterestedClients.
            // Consider extracting this common logic into class or something...
        }


        /// <summary>
        /// Teleport destination must be the first available node next to the dwelling 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private void LoadAndReply()
        {

            // Ajax call to load the region.
            ServerConnection connection = NetworkServer.Instance.Connections[this.ConnectionId];
            int[] regionsForLoading = new int[] { this.Rmsg.RegionId };

            string endpoint = "realms/{0}/regions";
            string[] @params = new string[] { connection.CurrentRealmId.ToString() };
            var queryParams = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < regionsForLoading.Length; i++)
            {
                queryParams.Add(new KeyValuePair<string, string>("regionIds", regionsForLoading[i].ToString()));
            }

            RequestManager.Instance.Get(endpoint, @params, queryParams, connection.Token, OnGetGetRegionsRequestFinished);
        }

        private void OnGetGetRegionsRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                string json = response.DataAsText;
                IList<Region> regions = JsonConvert.DeserializeObject<IList<Region>>(json);

                if (regions != null && regions.Count >= 1)
                {
                    foreach (var region in regions)
                    {
                        if (!NetworkServer.Instance.Regions.ContainsKey(region.Id))
                        {
                            NetworkServer.Instance.Regions.Add(region.Id, region);
                        }
                    }
                }

                this.Reply();
            }
            else
            {
                Debug.LogWarning("Error fetching teleport destination region from the API!");
            }
        }

        private Coord CalculateDestination()
        {
            var targetRegion = NetworkServer.Instance.Regions[this.Rmsg.RegionId];
            var targetDwelling = targetRegion.Dwellings.FirstOrDefault(d => d.Id == this.Rmsg.DwellingId);

            Coord freeNode = this.FindFreeNode(targetRegion, new Coord { X = targetDwelling.X, Y = targetDwelling.Y });

            return freeNode;
        }

        private Coord FindFreeNode(Region region, Coord dwellingPosition)
        {
            int[,] matrix = region.Matrix;
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
                        if (!HeroIsOnThisPosition(region.Heroes, checkPosition))
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
                        if (!HeroIsOnThisPosition(region.Heroes, checkPosition))
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
                        if (!HeroIsOnThisPosition(region.Heroes, checkPosition))
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
                        if (!HeroIsOnThisPosition(region.Heroes, checkPosition))
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

        private bool HeroIsOnThisPosition(IList<Hero> heroesInRegion, Coord checkPosition)
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