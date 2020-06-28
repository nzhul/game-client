using System;
using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class LogoutRequestHandler : IMessageHandler
    {
        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            // TODO: Do i need validation here ? Can this message be send by wrong user/hacker ?
            var connection = NetworkServer.Instance.Connections[connectionId];
            UIManager.Instance.OnDisconnect(connection);
            Debug.Log($"{connection.Username} logged out of the game!");
            NetworkServer.Instance.Connections.Remove(connectionId);
        }
    }
}
