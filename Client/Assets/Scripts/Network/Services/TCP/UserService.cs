using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Network.Shared.NetMessages.Users;
using Assets.Scripts.Shared.NetMessages.Users;

namespace Assets.Scripts.Network.Services.TCP
{
    public class UserService : IUserService
    {
        public void SendAuthRequest(int userId, string username, string token)
        {
            Net_AuthRequest msg = new Net_AuthRequest
            {
                UserId = userId,
                Username = username,
                Token = token
            };

            NetworkClient.Instance.SendServer(msg);
        }

        public void SendLogoutRequest()
        {
            var msg = new Net_LogoutRequest();
            NetworkClient.Instance.SendServer(msg);
        }
    }
}