using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Network.Shared.NetMessages.World.ClientServer;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.NetMessages.World.ClientServer;

namespace Assets.Scripts.Network.Services
{
    public class WorldService : IWorldService
    {
        public void FindOpponentRequest(CreatureType heroClass)
        {
            Net_FindOpponentRequest msg = new Net_FindOpponentRequest
            {
                Class = heroClass,
            };

            NetworkClient.Instance.SendServer(msg);
        }

        public void TeleportRequest(int armyId, int gameId, int dwellingId)
        {
            Net_TeleportRequest msg = new Net_TeleportRequest
            {
                ArmyId = armyId,
                GameId = gameId,
                DwellingId = dwellingId
            };

            NetworkClient.Instance.SendServer(msg);
        }

        //public void WorldEnterRequest(int id, int currentRealmId, int[] regionsForLoading)
        //{
        //    Net_WorldEnterRequest msg = new Net_WorldEnterRequest
        //    {
        //        UserId = id,
        //        CurrentRealmId = currentRealmId,
        //        RegionsForLoading = regionsForLoading
        //    };

        //    NetworkClient.Instance.SendServer(msg);
        //}

        public void CancelFindOpponentRequest()
        {
            Net_CancelFindOpponentRequest msg = new Net_CancelFindOpponentRequest();
            NetworkClient.Instance.SendServer(msg);
        }
    }
}
