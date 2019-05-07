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

            this.Heroes = new List<HeroView>();
        }
        #endregion

        public List<HeroView> Heroes { get; private set; }

        // TODO: Move everything related with map heroes here. Extract out of MapManager.
    }
}