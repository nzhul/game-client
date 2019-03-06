namespace Assets.Scripts.Network.MessageHandlers
{
    public interface IMessageHandler
    {
        void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input);
    }
}