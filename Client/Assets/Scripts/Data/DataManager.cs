using System;
using Assets.Scripts.Data.Models;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class DataManager : MonoBehaviour
    {
        private SaveData _saveData;
        private JsonSaver _jsonSaver;

        private static DataManager _instance;

        public static DataManager Instance
        {
            get
            {
                return _instance;
            }
        }

        public string Token
        {
            get
            {
                return _saveData.token;
            }

            set
            {
                _saveData.token = value;
            }
        }

        public int Id
        {
            get
            {
                return int.Parse(_saveData.id);
            }

            set
            {
                _saveData.id = value.ToString();
            }
        }

        public int CurrentRealmId
        {
            get
            {
                return int.Parse(_saveData.currentRealmId);
            }

            set
            {
                _saveData.currentRealmId = value.ToString();
            }
        }

        public string Username
        {
            get
            {
                return _saveData.username;
            }

            set
            {
                _saveData.username = value;
            }
        }

        public DateTime Created
        {
            get
            {
                return Convert.ToDateTime(_saveData.created);
            }

            set
            {
                //TODO: check if this saves correctly, else use -> ToLongDateString or other
                _saveData.created = value.ToString(); 
            }
        }

        public DateTime LastActive
        {
            get
            {
                return Convert.ToDateTime(_saveData.lastActive);
            }
            set
            {
                _saveData.lastActive = value.ToString();
            }
        }

        public string Gender
        {
            get
            {
                return _saveData.gender;
            }

            set
            {
                _saveData.gender = value;
            }
        }

        public int Age
        {
            get
            {
                return int.Parse(_saveData.age);
            }

            set
            {
                _saveData.age = value.ToString();
            }
        }

        public bool RememberMe
        {
            get
            {
                return Boolean.Parse(_saveData.rememberMe);
            }

            set
            {
                _saveData.rememberMe = value.ToString();
            }
        }

        public string Password
        {
            get
            {
                return _saveData.password;
            }

            set
            {
                _saveData.password = value;
            }
        }

        public UserAvatar Avatar
        {
            get
            {
                return _saveData.avatar;
            }
            set
            {
                _saveData.avatar = value;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;

                // for this to work the object must be on root level in the hierarchy
                // TODO: this might cause bugs since i will have two/three stacks of menus, each for each scene
                // DontDestroyOnLoad(gameObject);
            }

            _saveData = new SaveData();
            _jsonSaver = new JsonSaver();

            Instance.Load();
        }

        public void Save()
        {
            _jsonSaver.Save(_saveData);
        }

        public void Load()
        {
            _jsonSaver.Load(_saveData);
        }
    }
}
