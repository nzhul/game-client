using Assets.Scripts.LevelManagement;
using UnityEngine;

namespace Assets.Scripts.UI.HeroCreation
{
    public class HeroCreationManager : MonoBehaviour
    {
        #region Singleton
        private static HeroCreationManager _instance;

        public static HeroCreationManager Instance
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
        }
        #endregion
        public void OnBackBtnPressed()
        {
            LevelLoader.LoadLevel(LevelLoader.MAIN_MENU_SCENE);
        }
    }
}
