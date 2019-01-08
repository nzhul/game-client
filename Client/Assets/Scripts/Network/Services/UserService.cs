namespace Assets.Scripts.Network.Services
{
    public class UserService : IUserService
    {
        public void SendAuthRequest(int userId, string username, string token)
        {
            Net_AuthRequest msg = new Net_AuthRequest();
            msg.Id = userId;
            msg.Username = username;
            msg.Token = token;

            NetworkClient.Instance.SendServer(msg);
        }
    }
}