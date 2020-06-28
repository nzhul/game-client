namespace Assets.Scripts.Network.Services.TCP.Interfaces
{
    public interface IUserService
    {
        void SendAuthRequest(int userId, string username, string token);

        void SendLogoutRequest();
    }
}
