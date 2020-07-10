using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class DisconnectEventHandler
    {
        public void Handle(int connectionId)
        {
            // TODO:
            // 1. Logout the user and load MAIN_MENU
            // 2. Display "Server Down" warning!
            // 3. Reconnect the player and hide the server down message when the server is up!
            Debug.Log("We have been disconnected");
        }
    }
}