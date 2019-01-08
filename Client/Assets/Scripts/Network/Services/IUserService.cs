namespace Assets.Scripts.Network.Services
{
    public interface IUserService
    {
        void SendAuthRequest(int userId, string username, string token);
    }
}
