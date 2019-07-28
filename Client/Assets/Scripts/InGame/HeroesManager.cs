using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.InGame
{
    public class HeroesManager : MonoBehaviour
    {
        #region Singleton
        private static HeroesManager _instance;

        public static HeroesManager Instance
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

            this.Heroes = new Dictionary<int, HeroView>();
            this.NPCs = new Dictionary<int, NPCView>();
        }
        #endregion

        public Dictionary<int, HeroView> Heroes { get; private set; }

        public Dictionary<int, NPCView> NPCs { get; private set; }

        

        // TODO: Move everything related with map heroes here. Extract out of MapManager.
    }
}