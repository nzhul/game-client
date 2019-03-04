using Assets.Scripts.Network.Shared.NetMessages.World;

namespace Assets.Scripts.Network.Services
{
    public class WorldService : IWorldService
    {
        public void SendTeleportRequest(int heroId, int regionId, int dwellingId)
        {
            if (NetworkClient.Instance == null || !NetworkClient.Instance.IsStarted)
            {
                return;
            }

            Net_TeleportRequest msg = new Net_TeleportRequest
            {
                HeroId = heroId,
                RegionId = regionId,
                DwellingId = dwellingId
            };

            NetworkClient.Instance.SendServer(msg);
        }

        public void SendWorldEnterRequest(int id, int currentRealmId, int[] regionsForLoading)
        {
            if (NetworkClient.Instance == null || !NetworkClient.Instance.IsStarted)
            {
                return;
            }

            Net_WorldEnterRequest msg = new Net_WorldEnterRequest
            {
                UserId = id,
                CurrentRealmId = currentRealmId,
                RegionsForLoading = regionsForLoading
            };

            NetworkClient.Instance.SendServer(msg);
        }
    }
}
