using System;

namespace Assets.Scripts.RequestModels.Users.Input
{
    [Serializable]
    public class RegisterInput
    {
        public string username;

        public string email;

        public string password;

        public string confirmPassword;
    }
}
