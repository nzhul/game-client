namespace Assets.Scripts.Network.Services
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