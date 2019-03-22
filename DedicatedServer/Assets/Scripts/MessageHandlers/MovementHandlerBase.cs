using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.Shared.NetMessages.World.Models;
using BestHTTP;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
{
    public class MovementHandlerBase
    {
        protected void NotifyAllInterestedClients(Hero movingHero, int recievingHostId, NetMessage rmsg)
        {
            // 1. Get a list of all ServerConnections that have a region that contain our hero
            //    var connectionsToNotify = ...
            // 2. foreach connection -> SendClient(recievingHostId, clientConnectionId, rmsg);

            var interestedClients = NetworkServer.Instance.Connections.Where(c => c.Value.RegionIds.Any(r => r == movingHero.regionId));

            //Debug.Log("Sending Map movement update to clinets with connection id: " + string.Join(",", interestedClients.Select(x => x.Key.ToString())));

            foreach (var client in interestedClients)
            {
                NetworkServer.Instance.SendClient(recievingHostId, client.Key, rmsg);
            }
        }

        protected void UpdateCache(Hero cachedConnectionHero, Coord destination)
        {
            if (cachedConnectionHero != null)
            {
                cachedConnectionHero.x = destination.X;
                cachedConnectionHero.y = destination.Y;
            }

            var regionWithThatHero = NetworkServer.Instance.Regions.FirstOrDefault(r => r.Value.heroes.Any(h => h.id == cachedConnectionHero.id)).Value;
            if (regionWithThatHero != null)
            {
                var cachedHero = regionWithThatHero.heroes.FirstOrDefault(h => h.id == cachedConnectionHero.id);
                if (cachedHero != null)
                {
                    cachedHero.x = destination.X;
                    cachedHero.y = destination.Y;
                }
            }
        }

        protected void UpdateDatabase(int connectionId, int heroId, Coord destination)
        {
            string token = NetworkServer.Instance.Connections[connectionId].Token;
            string endpoint = "realms/heroes/{0}/{1}/{2}";
            string[] @params = new string[] { heroId.ToString(), destination.X.ToString(), destination.Y.ToString() };
            RequestManager.Instance.Put(endpoint, @params, token, OnUpdateHeroPosition);
        }

        private void OnUpdateHeroPosition(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                Debug.LogWarning("Error updating hero position in the API!");
            }
        }
    }
}
