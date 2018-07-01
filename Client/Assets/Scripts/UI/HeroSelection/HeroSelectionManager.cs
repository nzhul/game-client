using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.CharacterSelection
{
    public class HeroSelectionManager : MonoBehaviour
    {
        private const int maxHeroCount = 11;

        // Hero
        public RectTransform _heroContainer;
        public Button _heroBtnPrefab;
        public Image _emptySlotPrefab;

        public NamedSprite[] classIcons;

        // Selecter hero
        private int _selectedHeroId;
        public Color selectedHeroColor;
        public Color heroNormalColor;
        private Hero[] _heroList;

        #region Singleton
        private static HeroSelectionManager _instance;

        public static HeroSelectionManager Instance
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

        private void Start()
        {
            DataManager.Instance.Load(); //TODO: Delete this after development is complete

            InitializeHeroBtns();
        }

        public void OnBackBtnPressed()
        {
            LevelLoader.LoadLevel(LevelLoader.MAIN_MENU_SCENE);
        }

        private void InitializeHeroBtns()
        {
            Common.Empty(_heroContainer.transform);

            if (DataManager.Instance.Avatar != null && 
                DataManager.Instance.Avatar.heroes != null && 
                DataManager.Instance.Avatar.heroes.Length > 0)
            {
                _heroList = DataManager.Instance.Avatar.heroes;

                foreach (var hero in _heroList)
                {
                    Button heroBtn = GameObject.Instantiate<Button>(_heroBtnPrefab, _heroContainer);
                    heroBtn.name = hero.id + "_HeroBtn";
                    heroBtn.onClick.AddListener(delegate { OnHeroButtonPressed(hero.id); });

                    Text heroNameText = heroBtn.transform.Find("HeroName").GetComponent<Text>();
                    heroNameText.text = hero.name;

                    Image heroIcon = heroBtn.transform.Find("HeroIcon").GetComponent<Image>();
                    heroIcon.sprite = classIcons.FirstOrDefault(x => x.name == hero.@class).image;

                    Text heroDescriptionText = heroBtn.transform.Find("Description").GetComponent<Text>();
                    heroDescriptionText.text = "Level " + hero.level + " " + hero.@class;
                }

                int emptySlotsCount = maxHeroCount - _heroList.Length;

                for (int i = 0; i < emptySlotsCount; i++)
                {
                    GameObject.Instantiate<Image>(_emptySlotPrefab, _heroContainer);
                }
            }
        }

        private void OnHeroButtonPressed(int heroId)
        {
            Debug.Log(heroId);
        }
    }
}
