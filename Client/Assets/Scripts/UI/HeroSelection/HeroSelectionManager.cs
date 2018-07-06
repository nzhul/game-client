﻿using System;
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
        [Header("Hero selection panel:")]
        public RectTransform _heroContainer;
        public Button _heroBtnPrefab;
        public Image _emptySlotPrefab;
        public NamedSprite[] classIcons;
        private int _selectedHeroId;
        public Color selectedHeroColor;
        public Color heroNormalColor;
        private Hero[] _heroList;

        // Current hero info panel
        [Header("Current hero info panel:")]
        public Image currentHeroClassIcon;
        public Text currentHeroClassTypeText;
        public Text currentHeroStatistics;
        public NamedSprite[] classIconsTransparent;

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

            // TODO: Heroes should be sorted by lastActivity(lastPlayed) starting from top
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
                    heroBtn.onClick.AddListener(delegate { OnHeroButtonPressed(heroBtn, hero.id); });

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

        private void OnHeroButtonPressed(Button target, int heroId)
        {
            _selectedHeroId = heroId;

            foreach (Transform btnObject in _heroContainer.transform)
            {
                Button btn = btnObject.GetComponent<Button>() as Button;
                if (btn != null)
                {
                    ColorBlock cb1 = btn.colors;
                    cb1.normalColor = heroNormalColor;
                    btn.colors = cb1;
                }
            }

            HighlightButton(target, selectedHeroColor);

            UpdateHeroInfoPanel(heroId);

            UpdateEnvironmentAnd3DModel(heroId);
        }

        private void UpdateHeroInfoPanel(int heroId)
        {
            Hero selectedHero = DataManager.Instance.Avatar.heroes.FirstOrDefault(h => h.id == heroId);
            if (selectedHero != null)
            {
                currentHeroClassIcon.sprite = classIconsTransparent.FirstOrDefault(a => a.name == selectedHero.@class).image;
                currentHeroClassIcon.SetNativeSize();
                currentHeroClassTypeText.text = selectedHero.@class.ToUpper();

                //TODO: use textmeshPro and improve the UI.
                string infoTextTemplate = @"{0} - Level {1} {2}

Play time: {3}

?? dungeons cleared

Best monster: ??

Other statistics: ??";

                string playTimeText = string.Format("{0} hours {1} minutes", selectedHero.timePlayed.Hours, selectedHero.timePlayed.Minutes);

                string infoText = string.Format(infoTextTemplate, 
                    selectedHero.name, 
                    selectedHero.level, 
                    selectedHero.@class,
                    playTimeText);

                currentHeroStatistics.text = infoText;
            }
            else
            {
                Debug.LogWarning("Hero with id " + heroId + " is not loaded in DataManager!");
            }
        }

        private void UpdateEnvironmentAnd3DModel(int heroId)
        {
            // TODO ...
        }

        private void HighlightButton(Button button, Color color)
        {
            ColorBlock cb = button.colors;
            cb.normalColor = color;
            button.colors = cb;
        }
    }
}