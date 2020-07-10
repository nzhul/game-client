﻿using System;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Shared.NetMessages.Users;
using BestHTTP;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class AuthRequestHandler : IMessageHandler
    {
        public static event Action<ServerConnection> OnAuth;

        /// <summary>
        /// The user will authenticate directly throu the API.
        /// Then he will send his token and username to the dedicated server.
        /// This way we can track his connection and online status.
        /// </summary>
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            Net_AuthRequest msg = (Net_AuthRequest)input;
            Net_OnAuthRequest rmsg = new Net_OnAuthRequest();

            if (msg.IsValid())
            {
                rmsg.Success = 1;
                rmsg.ConnectionId = connectionId;

                ServerConnection connection = new ServerConnection
                {
                    ConnectionId = connectionId,
                    UserId = msg.UserId,
                    Token = msg.Token,
                    Username = msg.Username,
                    MMR = msg.MMR
                };

                NetworkServer.Instance.Connections.Add(connectionId, connection);

                string endpoint = "users/{0}/setonline/{1}";
                string[] @params = new string[] { msg.UserId.ToString(), connectionId.ToString() };

                RequestManager.Instance.Put(endpoint, @params, msg.Token, OnSetOnline);

                Debug.Log(string.Format("{0} logged in to the server!", msg.Username));
                OnAuth?.Invoke(connection);
            }
            else
            {
                rmsg.Success = 0;
                rmsg.ErrorMessage = "Invalid connection request!";
            }

            NetworkServer.Instance.SendClient(recievingHostId, connectionId, rmsg);
        }

        private void OnSetOnline(HTTPRequest request, HTTPResponse response)
        {
            if (!NetworkCommon.RequestIsSuccessful(request, response, out string errorMessage))
            {
                Debug.LogWarning("Error setting user as online in the API!");
            }
        }
    }
}