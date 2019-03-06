using UnityEngine;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class DisconnectEventHandler
    {
        public void Handle(int connectionId)
        {
            Debug.Log("We have been disconnected");
        }
    }
}