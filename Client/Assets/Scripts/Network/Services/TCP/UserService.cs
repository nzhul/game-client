using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Network.Shared.NetMessages.Users;

namespace Assets.Scripts.Network.Services.TCP
{
    public class UserService : IUserService
    {
        public void SendAuthRequest(int userId, string username, string token)
        {
            Net_AuthRequest msg = new Net_AuthRequest
            {
                Id = userId,
                Username = username,
                Token = token
            };

            NetworkClient.Instance.SendServer(msg);
        }
    }
}