namespace Assets.Scripts.MessageHandlers
{
    public class WorldEnterRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_WorldEnterRequest msg = (Net_WorldEnterRequest)input;
            Net_OnWorldEnter rmsg = new Net_OnWorldEnter();

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
}
