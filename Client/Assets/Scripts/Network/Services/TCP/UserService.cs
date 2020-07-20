using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.Network.Services.TCP.Interfaces;
using Assets.Scripts.Network.Shared.NetMessages.Users;
using Assets.Scripts.Shared.NetMessages.Users;

namespace Assets.Scripts.Network.Services.TCP
{
    public class UserService : IUserService
    {
        public void SendAuthRequest(LoginResponse loginInfo)
        {
            Net_AuthRequest msg = new Net_AuthRequest
            {
                UserId = loginInfo.user.id,
                Username = loginInfo.user.username,
                Token = loginInfo.tokenString,
                GameId = loginInfo.user.gameId,
                BattleId = loginInfo.user.battleId
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