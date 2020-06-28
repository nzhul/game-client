using System;
using Assets.Scripts.Data;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Shared.NetMessages.Users;
using Assets.Scripts.UI.Modals.MainMenuModals;
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
                RequestManagerHttp.Instance.SetToken(DataManager.Instance.Token);
                GameManager.Instance.LoadScene(LevelLoader.LOBBY_SCENE, false);
            }

            Debug.Log("Client has authenticated to dedicated server!");
        }
    }
}