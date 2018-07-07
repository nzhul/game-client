using System;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.HeroCreation
{
    public class HeroCreationManager : MonoBehaviour
    {
        private string _selectedFaction = "sanctuary";
        private string _selectedHeroClass = "knight";

        public FactionData[] factionData;

        [Header("Hero class selection:")]
        public RectTransform _classContainer;
        public Button _classBtnPrefab;
        public Color selectedHeroColor;
        public Color heroNormalColor;

        [Header("Current hero class info panel:")]
        public Image currentClassIcon;
        public Text currentClassTypeText;
        public Text currentClassDescription;

        private void Start()
        {
            InitializeClassButtons();
        }

        private void InitializeClassButtons()
        {
            Common.Empty(_classContainer.transform);

            foreach (var @class in factionData[0].heroes)
            {
                Button classBtn = GameObject.Instantiate<Button>(_classBtnPrefab, _classContainer);
                classBtn.name = @class.name + "_HeroBtn";
                classBtn.onClick.AddListener(delegate { OnClassButtonPressed(classBtn, @class.name); });

                Text classNameText = classBtn.transform.Find("ClassName").GetComponent<Text>();
                classNameText.text = @class.name.ToUpper();
            }
        }

        private void OnClassButtonPressed(Button heroBtn, string name)
        {
            
        }


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

        public void OnSanctuaryBtnPressed()
        {
            // populate the icon, title and description of faction panel
            // switch to sanctuary heroes panel and select (click) the first one - Knight
            // deselect underworldBtn
            // mark sanctuaryBtn as selected
        }

        public void OnUnderworldBtnPressed()
        {
            // populate the icon, title and description of faction panel
            // switch to underworld heroes panel and select (click) the first one - Warlord
            // deselect underworldBtn
            // mark sanctuaryBtn as selected
        }

        public void OnCreateBtnPressed()
        {
            // validate the input client-side
            // do api call to create new hero
            // if the user dont have avatar in this realm - create avatar, then create hero
            // check if username is taken and return an error if is already used by other player
            // usernames are unique per realm.

            // on success avatar/hero creation - save the avatar/hero information in DataManager.
            // load heroSelectionScene and let the user enter the realm with his new hero.
        }

        private void OnHeroClassBtnPressed(string heroClass)
        {
            // switch the 3D model
            // populate the icon, title and description of hero panel
            // deselect other heroClass buttons
            // mark this btn as selected
        }

    }
}
