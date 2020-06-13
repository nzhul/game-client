using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Shared.Models;
using Assets.Scripts.Shared.Models.Units;
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

        public Unit GetUnitById(int heroId, int unitId, PlayerType playerType)
        {
            if (playerType == PlayerType.Human)
            {
                var hero = this.Heroes[heroId].rawUnit as Hero;
                return hero.Units.FirstOrDefault(u => u.Id == unitId);
            }
            else
            {
                return this.NPCs[heroId].npc.Units.FirstOrDefault(u => u.Id == unitId);
            }
        }

        // TODO: Move everything related with map heroes here. Extract out of MapManager.
    }
}