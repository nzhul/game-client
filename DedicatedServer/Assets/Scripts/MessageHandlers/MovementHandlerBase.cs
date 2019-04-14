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
        //protected void NotifyClientsInCurrentHeroRegion(Hero movingHero, int recievingHostId, NetMessage rmsg)
        //{
        //    this.NotifyClientsInRegion(movingHero.regionId, recievingHostId, rmsg);
        //}

        //protected void NotifyClientsInOldHeroRegion(int oldRegionId, int recievingHostId, NetMessage rmsg)
        //{
        //    this.NotifyClientsInRegion(oldRegionId, recievingHostId, rmsg);
        //}

        protected void NotifyClientsInRegion(int regionId, int recievingHostId, NetMessage rmsg)
        {
            var interestedClients = NetworkServer.Instance.Connections.Where(c => c.Value.RegionIds.Any(r => r == regionId));

            foreach (var client in interestedClients)
            {
                NetworkServer.Instance.SendClient(recievingHostId, client.Key, rmsg);
            }
        }


        protected void UpdateCache(Hero cachedConnectionHero, Coord destination, int regionId)
        {
            if (cachedConnectionHero != null)
            {
                cachedConnectionHero.x = destination.X;
                cachedConnectionHero.y = destination.Y;
                cachedConnectionHero.regionId = regionId;
            }

            var regionWithThatHero = NetworkServer.Instance.Regions.FirstOrDefault(r => r.Value.heroes.Any(h => h.id == cachedConnectionHero.id)).Value;
            if (regionWithThatHero != null)
            {
                var cachedHero = regionWithThatHero.heroes.FirstOrDefault(h => h.id == cachedConnectionHero.id);
                if (cachedHero != null)
                {
                    cachedHero.x = destination.X;
                    cachedHero.y = destination.Y;
                    cachedHero.regionId = regionId;
                }
            }
        }

        protected void UpdateDatabase(int connectionId, int heroId, Coord destination, int regionId)
        {
            string token = NetworkServer.Instance.Connections[connectionId].Token;
            string endpoint = "realms/heroes/{0}/{1}/{2}/{3}";
            string[] @params = new string[] { regionId.ToString(), heroId.ToString(), destination.X.ToString(), destination.Y.ToString() };
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
