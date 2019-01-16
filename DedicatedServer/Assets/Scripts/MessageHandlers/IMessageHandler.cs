namespace Assets.Scripts.MessageHandlers
{
    public interface IMessageHandler
    {
        void Handle(int connectionId, int channelId, int recievingHostId, NetMessage input);
    }
}
