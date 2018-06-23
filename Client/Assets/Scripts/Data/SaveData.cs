using System;

namespace Assets.Scripts.Data
{
    [Serializable]
    public class SaveData
    {
        private readonly string defaultUsername = "Unknown";

        public string token;
        public string id;
        public string currentRealmId;
        public string username;
        public string created;
        public string lastActive;
        public string gender;
        public string age;
        public string rememberMe;
        public string password;

        public string hashValue;

        public SaveData()
        {
            token = string.Empty;
            id = string.Empty;
            currentRealmId = string.Empty;
            username = defaultUsername;
            created = string.Empty;
            lastActive = string.Empty;
            gender = string.Empty;
            age = string.Empty;
            hashValue = string.Empty;
            rememberMe = "false";
            password = string.Empty;
        }
    }
}
