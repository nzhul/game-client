using System;
using System.Collections.Generic;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Shared.Models;

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
        public string activeHeroId;
        public int activeGameId;
        public Guid? activeBattleId;
        public Avatar avatar;
        public Game activeGame;
        public BattleData battleData;
        public Dictionary<CreatureType, UnitConfiguration> unitConfigurations;

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
            avatar = new Avatar();
            battleData = new BattleData();
            activeBattleId = null;
        }
    }
}
