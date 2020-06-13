using System;
using System.Linq;
using Assets.Scripts.Network.Matchmaking;
using Assets.Scripts.Network.Services;
using BestHTTP;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class DisconnectEventHandler
    {
        public static event Action<ServerConnection> OnDisconnect;
        public static event Action<int> OnRegionUnload;

        public void Handle(int connectionId)
        {
            if (NetworkServer.Instance.Connections.ContainsKey(connectionId))
            {
                ServerConnection connection = NetworkServer.Instance.Connections[connectionId];
                Debug.Log(string.Format("{0} has disconnected from the server!", connection.Username));

                string endpoint = "users/{0}/setoffline";
                string[] @params = new string[] { connection.UserId.ToString() };

                RequestManager.Instance.Put(endpoint, @params, connection.Token, OnSetOffline);


                Matchmaker.Instance.UnRegisterPlayer(connection);
                NetworkServer.Instance.Connections.Remove(connectionId);

                OnDisconnect?.Invoke(connection);

                // RemoveUnusedRegions(connection);

                // TODO: remove all this.Regions that are not currently used
                // When user disconnects check if he is the only one using this realm
                // if yes -> remove the region from the collection!
            }
            else
            {
                Debug.Log("Someone disconnected from the server!");
            }
        }

        private void RemoveUnusedRegions(ServerConnection connection)
        {
            var userRegions = connection.RegionIds;

            if (userRegions != null && userRegions.Length > 0)
            {
                for (int i = 0; i < userRegions.Length; i++)
                {
                    int regionId = userRegions[i];
                    bool anyUsersWithThisRegion = NetworkServer.Instance.Connections.Any(c => c.Value.RegionIds.Any(r => r == regionId));

                    if (!anyUsersWithThisRegion)
                    {
                        Debug.Log($"Region with Id {regionId} unloaded!");
                        NetworkServer.Instance.Regions.Remove(regionId);
                        OnRegionUnload?.Invoke(regionId);
                    }
                }
            }
        }

        public static void InvokeRegionUnloadEvent(int regionId)
        {
            OnRegionUnload?.Invoke(regionId);
        }

        private void OnSetOffline(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                Debug.LogWarning("Error setting user as offline in the API!");
            }
        }
    }
}
