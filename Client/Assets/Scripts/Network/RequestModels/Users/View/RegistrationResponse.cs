#pragma warning disable 0649 // this one disables -- never used variable in unity editor
using System;

namespace Assets.Scripts.Network.RequestModels.Users.View
{
    [Serializable]
    public class RegistrationResponse
    {

        public string id;

        public string username;

        private string created;

        private string lastActive;

        public string email;

        public DateTime? dateCreated
        {
            get { return Convert.ToDateTime(created); }
        }

        public DateTime? dateLastActive
        {
            get { return Convert.ToDateTime(lastActive); }
        }
    }
}
