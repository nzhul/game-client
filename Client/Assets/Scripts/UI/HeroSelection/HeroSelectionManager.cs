//using System.Collections.Generic;
//using System.Linq;
//using Assets.Scripts.Data;
//using Assets.Scripts.LevelManagement;
//using Assets.Scripts.Shared.Models;
//using Assets.Scripts.UI.HeroSelection;
//using Assets.Scripts.Utilities;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Assets.Scripts.UI.CharacterSelection
//{
//    public class HeroSelectionManager : MonoBehaviour
//    {
//        private const int maxHeroCount = 11;

//        // Hero
//        [Header("Hero selection panel:")]
//        public RectTransform _heroContainer;
//        public Button _heroBtnPrefab;
//        public Image _emptySlotPrefab;
//        public NamedSprite[] classIcons;
//        public int selectedHeroId;
//        public Color selectedHeroColor;
//        public Color heroNormalColor;
//        private IList<Hero> _heroList;

//        // Current hero info panel
//        [Header("Current hero info panel:")]
//        public Image currentHeroClassIcon;
//        public Text currentHeroClassTypeText;
//        public Text currentHeroStatistics;
//        public NamedSprite[] classIconsTransparent;

//        [Header("3D Model:")]
//        public Text model3DText;
//        public Text heroNameText;

//        #region Singleton
//        private static HeroSelectionManager _instance;

//        public static HeroSelectionManager Instance
//        {
//            get
//            {
//                return _instance;
//            }
//        }

//        private void Awake()
//        {
//            if (_instance != null)
//            {
//                Destroy(gameObject);
//            }
//            else
//            {
//                _instance = this;
//            }
//        }
//        #endregion

//        private void Start()
//        {
//            //DataManager.Instance.Load(); //TODO: Delete this after development is complete

//            // TODO: Heroes should be sorted by lastActivity(lastPlayed) starting from top

//            // TODO: if current hero count = 11 -> make the button disabled.
//            // user will have a maximum of 11 heroes per realm
//            InitializeHeroBtns();
//        }

//        public void OnBackBtnPressed()
//        {
//            LevelLoader.LoadLevel(LevelLoader.MAIN_MENU_SCENE);
//        }

//        public void OnEnterRealmBtnPressed()
//        {
//            DataManager.Instance.Save(); // saving the activeHeroId
//            LevelLoader.LoadLevel(LevelLoader.GAME_SCENE);
//        }

//        public void OnDeleteBtnPressed()
//        {
//            ConfirmDeleteModal.Instance.Open();
//        }

//        public void OnCreateBtnPressed()
//        {
//            LevelLoader.LoadLevel(LevelLoader.HERO_CREATION_SCENE);
//        }

//        public void InitializeHeroBtns()
//        {
//            Common.Empty(_heroContainer.transform);

//            if (DataManager.Instance != null &&
//                DataManager.Instance.Avatar != null &&
//                DataManager.Instance.Avatar.Heroes != null &&
//                DataManager.Instance.Avatar.Heroes.Count > 0)
//            {
//                _heroList = DataManager.Instance.Avatar.Heroes;

//                int index = 0;
//                foreach (Hero hero in _heroList)
//                {
//                    Button heroBtn = GameObject.Instantiate<Button>(_heroBtnPrefab, _heroContainer);
//                    heroBtn.name = hero.Id + "_HeroBtn";
//                    heroBtn.onClick.AddListener(delegate { OnHeroButtonPressed(heroBtn, hero.Id); });

//                    Text heroNameText = heroBtn.transform.Find("HeroName").GetComponent<Text>();
//                    heroNameText.text = hero.Class.ToString();

//                    Image heroIcon = heroBtn.transform.Find("HeroIcon").GetComponent<Image>();
//                    heroIcon.sprite = classIcons.FirstOrDefault(x => x.name == hero.Class.ToString()).image;

//                    Text heroDescriptionText = heroBtn.transform.Find("Description").GetComponent<Text>();
//                    heroDescriptionText.text = "Level " + hero.Level + " " + hero.Class;

//                    if (index == 0)
//                    {
//                        //HighlightButton(heroBtn, selectedHeroColor);
//                        heroBtn.onClick.Invoke();
//                    }

//                    index++;
//                }

//                int emptySlotsCount = maxHeroCount - _heroList.Count;

//                for (int i = 0; i < emptySlotsCount; i++)
//                {
//                    GameObject.Instantiate<Image>(_emptySlotPrefab, _heroContainer);
//                }
//            }
//        }

//        private void OnHeroButtonPressed(Button target, int heroId)
//        {
//            selectedHeroId = heroId;
//            DataManager.Instance.ActiveHeroId = selectedHeroId;

//            Common.HighlightButton(target, selectedHeroColor, heroNormalColor, _heroContainer);

//            UpdateHeroInfoPanel(heroId);

//            UpdateEnvironmentAnd3DModel(heroId);
//        }

//        private void UpdateHeroInfoPanel(int heroId)
//        {
//            Hero selectedHero = DataManager.Instance.Avatar.Heroes.FirstOrDefault(h => h.Id == heroId);
//            if (selectedHero != null)
//            {
//                currentHeroClassIcon.sprite = classIconsTransparent.FirstOrDefault(a => a.name == selectedHero.Class.ToString()).image;
//                currentHeroClassIcon.SetNativeSize();
//                currentHeroClassTypeText.text = selectedHero.Class.ToString().ToUpper();

//                //TODO: use textmeshPro and improve the UI.
//                string infoTextTemplate = @"{0} - Level {1} {2}

//?? dungeons cleared

//Best monster: ??

//Other statistics: ??";

//                string infoText = string.Format(infoTextTemplate,
//                    "HeroName",
//                    selectedHero.Level,
//                    selectedHero.Class);

//                currentHeroStatistics.text = infoText;
//            }
//            else
//            {
//                Debug.LogWarning("Hero with id " + heroId + " is not loaded in DataManager!");
//            }
//        }

//        private void UpdateEnvironmentAnd3DModel(int heroId)
//        {
//            // TODO ...
//            Hero hero = _heroList.FirstOrDefault(x => x.Id == heroId);
//            model3DText.text = "HeroName";
//            heroNameText.text = "HeroName";
//        }
//    }
//}
