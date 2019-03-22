using System;

namespace Assets.Scripts.Network.MessageHandlers
{
    public class OnWorldEnterRequestHandler : IMessageHandler
    {
        public static event Action<Net_OnWorldEnter> OnWorldEnter;

        public void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input)
        {
            //1. Raise event
            var msg = (Net_OnWorldEnter)input;
            OnWorldEnter?.Invoke(msg);

            //2. Enable player inputs

            if (msg.Success == 1)
            {
                PlayerController.Instance.EnableInputs();
            }
        }
    }
}