using System;
using System.Collections.Generic;
using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Shared.NetMessages.World;
using BestHTTP;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
{
    public class MapMovementRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_MapMovementRequest msg = (Net_MapMovementRequest)input;
            Net_OnMapMovement rmsg = new Net_OnMapMovement();

            
            // 1. Validate the new position
            if (IsNewPositionValid(msg))
            {
                rmsg.Success = 1;
                rmsg.HeroUpdates = new List<HeroUpdate>
                {
                    new HeroUpdate
                    {
                        HeroId = msg.HeroId,
                        NewX = msg.NewX,
                        NewY = msg.NewY
                    }
                };

                // 2. Notify the requester
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);

                // 3. Notify the interested clients ( must exclude the requester )
                NotifyAllInterestedClients();

                // 4. Update the database
                UpdateDatabase(connectionId, msg);
            }
            else
            {
                rmsg.Error = "Requested position is not valid!";
                rmsg.Success = 0;
                NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
            }
        }

        private static void UpdateDatabase(int connectionId, Net_MapMovementRequest msg)
        {
            string token = NetworkServer.Instance.Connections[connectionId].Token;
            string endpoint = "realms/heroes/{0}/{1}/{2}";
            string[] @params = new string[] { msg.HeroId.ToString(), msg.NewX.ToString(), msg.NewY.ToString() };
            RequestManager.Instance.Put(endpoint, @params, token, OnUpdateHeroPosition);
        }

        private static void OnUpdateHeroPosition(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                Debug.LogWarning("Error updating hero position in the API!");
            }
        }

        private void NotifyAllInterestedClients()
        {
            // 1. Get a list of all ServerConnections that have a region that contain our hero
            //    var connectionsToNotify = ...
            // 2. foreach connection -> SendClient(recievingHostId, clientConnectionId, rmsg);
        }

        private bool IsNewPositionValid(Net_MapMovementRequest msg)
        {
            // TODO: Implement the validation
            return true;
        }
    }
}
