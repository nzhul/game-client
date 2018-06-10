using System;
using UnityEngine;

namespace Assets.Scripts.RequestModels.Users.View
{
    [Serializable]
    public class LoginResponse
    {
        public string tokenString;
        public User user;
    }

    [Serializable]
    public class User
    {
        public int id;
        public string username;
        public string gender;
        public int age;

        [SerializeField]
        private string created;

        [SerializeField]
        private string lastActive;

        public string interests;
        public string city;
        public string country;
        public string photoUrl;

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
