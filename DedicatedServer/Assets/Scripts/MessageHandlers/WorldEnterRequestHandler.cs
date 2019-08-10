using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Shared.DataModels;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
{
    public class WorldEnterRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_WorldEnterRequest msg = (Net_WorldEnterRequest)input;

            InnerWERHandler innerHandler = new InnerWERHandler();
            innerHandler.Handle(connectionId, channelId, recievingHostId, msg);

            // 1. Load user avatar via API call

            // 2. Store msg.CurrentRealmId somewhere ( ServerConnection entity maybe )

            // 3. Load msg.RegionsForLoading if they are not already loaded by other player!

            // 4. When all 3 steps are completed -> Send Message to the client that the server is ready
            // which means that the client can proceed with the initialization of the map.!
            // !! OR NOT --> maybe we can let the user enter the world without waiting for the server.
            // We can let the user play and do ASYNC checks with the server if his actions are legal.
            // If at some point we detect that the player has performed an illegal action -> we disconnect him from the server
        }
    }

    public class InnerWERHandler
    {
        public static event Action<Region> OnRegionLoaded;

        public int ConnectionId { get; set; }

        public ServerConnection Connection { get; set; }

        public Net_WorldEnterRequest Msg { get; set; }

        public int RecievingHostId { get; set; }

        public void Handle(int connectionId, int channelId, int recievingHostId, Net_WorldEnterRequest msg)
        {
            ServerConnection connection = NetworkServer.Instance.Connections[connectionId];
            connection.CurrentRealmId = msg.CurrentRealmId;

            Connection = connection;
            ConnectionId = connectionId;
            Msg = msg;
            RecievingHostId = recievingHostId;

            LoadUserAvatar(connectionId, msg);
        }

        private void LoadUserAvatar(int connectionId, Net_WorldEnterRequest msg)
        {
            string[] @params = new string[] { Connection.CurrentRealmId.ToString(), Connection.UserId.ToString() };
            string avatarEndpoint = "realms/{0}/users/{1}/avatar";
            RequestManager.Instance.Get(avatarEndpoint, @params, Connection.Token, OnGetUserAvatarRequestFinished);
        }

        private void OnGetUserAvatarRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                string json = response.DataAsText;
                UserAvatar userAvatar = JsonConvert.DeserializeObject<UserAvatar>(json);

                if (userAvatar != null)
                {
                    Connection.Avatar = userAvatar;

                    LoadUserRegions();
                }
            }
            else
            {
                // ???
            }
        }


        private void LoadUserRegions()
        {
            int[] regionsForLoading = Connection.Avatar.Heroes.Select(h => h.RegionId).ToArray();
            regionsForLoading = regionsForLoading.Where(r => !NetworkServer.Instance.Regions.Any(rr => rr.Key == r)).ToArray();

            if (regionsForLoading.Length == 0)
            {
                // All regions are already loaded
                Net_OnWorldEnter rmsg = new Net_OnWorldEnter
                {
                    Success = 1
                };
                // Debug.Log("Sending OnWorldEnter msg with Success: " + rmsg.Success);
                NetworkServer.Instance.SendClient(RecievingHostId, ConnectionId, rmsg);
            }
            else
            {
                string endpoint = "realms/{0}/regions";
                string[] @params = new string[] { Connection.CurrentRealmId.ToString() };
                List<KeyValuePair<string, string>> queryParams = new List<KeyValuePair<string, string>>();

                for (int i = 0; i < regionsForLoading.Length; i++)
                {
                    queryParams.Add(new KeyValuePair<string, string>("regionIds", regionsForLoading[i].ToString()));
                }

                RequestManager.Instance.Get(endpoint, @params, queryParams, Connection.Token, OnGetGetRegionsRequestFinished);
            }
        }

        private void OnGetGetRegionsRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            Net_OnWorldEnter rmsg = new Net_OnWorldEnter();
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
                            OnRegionLoaded?.Invoke(region);
                        }
                    }
                }

                rmsg.Success = 1;
            }
            else
            {
                rmsg.Success = 0;
                rmsg.ErrorMessage = "Error fetching User regions from the API";
            }

            // Debug.Log("Sending OnWorldEnter msg with Success: " + rmsg.Success);
            NetworkServer.Instance.SendClient(RecievingHostId, ConnectionId, rmsg);
        }
    }
}
