using System;
using Assets.Scripts.Shared.NetMessages;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnAuthRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnAuthRequest> OnAuthRequest;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            var msg = (Net_OnAuthRequest)input;

            if (OnAuthRequest != null)
            {
                OnAuthRequest(msg);
            }

            Debug.Log("Client has authenticated to dedicated server!");
        }
    }
}