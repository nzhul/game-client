using Assets.Scripts.Network.Shared.NetMessages.World;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Scripts.MessageHandlers
{
    public class TeleportRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {

            Net_TeleportRequest msg = (Net_TeleportRequest)input;

            Debug.Log("Handling new Teleport request: " + JsonConvert.SerializeObject(msg, Formatting.Indented));
        }
    }
}