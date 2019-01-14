namespace Assets.Scripts.Network.Services
{
    public class WorldService : IWorldService
    {
        public void SendWorldEnterRequest(int id, int currentRealmId, int[] regionsForLoading)
        {
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
