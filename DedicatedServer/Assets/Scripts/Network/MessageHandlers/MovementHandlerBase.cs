using System.Linq;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Shared.Models;
using BestHTTP;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class MovementHandlerBase
    {
        protected void NotifyClientsInGame(int gameId, int recievingHostId, NetMessage rmsg)
        {
            var interestedClients = NetworkServer.Instance.Connections.Where(c => c.Value.GameId == gameId);

            foreach (var client in interestedClients)
            {
                NetworkServer.Instance.SendClient(recievingHostId, client.Key, rmsg);
            }
        }


        protected void UpdateCache(Army army, Coord destination, int regionId)
        {
            if (army != null)
            {
                army.X = destination.X;
                army.Y = destination.Y;
                army.GameId = regionId;
            }
        }

        //protected void UpdateDatabase(int connectionId, int armyId, Coord destination, int regionId)
        //{
        //    RequestManagerHttp.ArmiesService.UpdateArmyPosition(armyId, destination.X, destination.Y);

        //    //string token = NetworkServer.Instance.Connections[connectionId].Token;
        //    //string endpoint = "realms/heroes/{0}/{1}/{2}/{3}";
        //    //string[] @params = new string[] { regionId.ToString(), armyId.ToString(), destination.X.ToString(), destination.Y.ToString() };
        //    //RequestManager.Instance.Put(endpoint, @params, token, OnUpdateHeroPosition);
        //}

        private void OnUpdateHeroPosition(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                Debug.LogWarning("Error updating hero position in the API!");
            }
        }
    }
}
