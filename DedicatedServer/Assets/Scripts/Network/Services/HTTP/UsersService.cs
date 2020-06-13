using Assets.Scripts.Configuration;
using Assets.Scripts.Network.Services.HTTP.Interfaces;

namespace Assets.Scripts.Network.Services.HTTP
{
    public class UsersService : BaseService, IUsersService
    {
        public User GetUser(int id)
        {
            return base.Get<User>($"users/" + id);
        }
    }
}
