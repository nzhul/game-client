using Assets.Scripts.LevelManagement;
using Assets.Scripts.Shared.NetMessages;
using Assets.Scripts.UI.Modals.MainMenuModals;
using System;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnAuthRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnAuthRequest> OnAuthRequest;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            var msg = (Net_OnAuthRequest)input;

            OnAuthRequest?.Invoke(msg);

            LoginModal.Instance.EnableForm();

            if (msg.Success == 1)
            {
                LevelLoader.LoadLevel(LevelLoader.LOBBY_SCENE);
            }

            Debug.Log("Client has authenticated to dedicated server!");
        }
    }
}