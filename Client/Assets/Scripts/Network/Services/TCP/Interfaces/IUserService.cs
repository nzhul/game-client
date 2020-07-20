using Assets.Scripts.Network.RequestModels.Users.View;

namespace Assets.Scripts.Network.Services.TCP.Interfaces
{
    public interface IUserService
    {
        void SendAuthRequest(LoginResponse loginInfo);

        void SendLogoutRequest();
    }
}
