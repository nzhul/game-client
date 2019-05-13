using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InGame
{
    public class MonstersManager : MonoBehaviour
    {
        #region Singleton
        private static MonstersManager _instance;

        public static MonstersManager Instance
        {
            get
            {
                return _instance;
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
            }

            this.Monsters = new Dictionary<int, MonsterView>();
        }
        #endregion

        public Dictionary<int, MonsterView> Monsters { get; private set; }
    }
}
