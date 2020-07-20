using System.Collections.Generic;
using Assets.Scripts.InGame.Map.Entities;
using UnityEngine;

namespace Assets.Scripts.InGame
{
    public class EntitiesManager : MonoBehaviour
    {
        #region Singleton
        private static EntitiesManager _instance;

        public static EntitiesManager Instance
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

            this.Entities = new Dictionary<int, EntityView>();
            //this.NPCs = new Dictionary<int, NPCView>();
        }
        #endregion

        public Dictionary<int, EntityView> Entities { get; private set; }

        //public Dictionary<int, NPCView> NPCs { get; private set; }

        //public Unit GetUnitById(int armyId, int unitId, PlayerType playerType)
        //{
        //    if (playerType == PlayerType.Human)
        //    {
        //        return this.Entities[armyId].rawUnit;
        //    }
        //    else
        //    {
        //        return this.NPCs[armyId].npc.Units.FirstOrDefault(u => u.Id == unitId);
        //    }
        //}

        // TODO: Move everything related with map heroes here. Extract out of MapManager.
    }
}