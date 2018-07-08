using System;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Network;
using Assets.Scripts.Network.RequestModels.Realms;
using Assets.Scripts.UI.Modals.MainMenuModals;
using Assets.Scripts.Utilities;
using BestHTTP;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.HeroCreation
{
    public class HeroCreationManager : MonoBehaviour
    {
        private string _selectedFaction = "Sanctuary";
        private string _selectedHeroClass = "Knight";

        public FactionData[] factionData;

        [Header("Faction selection:")]
        public RectTransform _factionContainer;
        public Button _factionBtnPrefab;
        public Color selectedFactionColor;
        public Color factionNormalColor;

        [Header("Hero class selection:")]
        public RectTransform _classContainer;
        public Button _classBtnPrefab;
        public Color selectedHeroColor;
        public Color heroNormalColor;

        [Header("Faction info panel:")]
        public Image currentFactionIcon;
        public Text currentFactionTypeText;
        public Text currentFactionDescription;

        [Header("Class info panel:")]
        public Image currentClassIcon;
        public Text currentClassTypeText;
        public Text currentClassDescription;

        [Header("3D Model:")]
        public Text model3DText;

        [Header("Username input:")]
        public InputField usernameInputField;
        public string userNameRegexPattern = "^[a-zA-Z0-9]+$";
        public Transform errorLabel;
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.linear;
        public Button createBtn;

        private void Start()
        {
            InitializeFactionButtons(_selectedFaction);
        }

        private void InitializeFactionButtons(string selectedFaction)
        {
            Common.Empty(_factionContainer.transform);

            int index = 0;
            foreach (var faction in factionData)
            {
                Button factionBtn = GameObject.Instantiate<Button>(_factionBtnPrefab, _factionContainer);
                factionBtn.name = faction.name + "_FactionBtn";
                factionBtn.onClick.AddListener(delegate { OnFactionBtnPressed(faction, faction.name, factionBtn); });

                Text factionNameText = factionBtn.transform.Find("FactionName").GetComponent<Text>();
                factionNameText.text = faction.name.ToUpper();

                if (index == 0)
                {
                    factionBtn.onClick.Invoke();
                }

                index++;
            }
        }

        private void InitializeClassButtons(string selectedFaction)
        {
            Common.Empty(_classContainer.transform);

            FactionData faction = factionData.FirstOrDefault(f => f.name == selectedFaction);

            int index = 0;
            foreach (var @class in faction.heroes)
            {
                Button classBtn = GameObject.Instantiate<Button>(_classBtnPrefab, _classContainer);
                classBtn.name = @class.name + "_HeroBtn";
                classBtn.onClick.AddListener(delegate { OnHeroClassBtnPressed(faction, @class.name, classBtn); });

                Text classNameText = classBtn.transform.Find("ClassName").GetComponent<Text>();
                classNameText.text = @class.name.ToUpper();

                if (index == 0)
                {
                    classBtn.onClick.Invoke();
                }

                index++;
            }
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

        public void OnCreateBtnPressed()
        {
            // validate the input client-side
            // do api call to create new hero
            // if the user dont have avatar in this realm - create avatar, then create hero
            // check if username is taken and return an error if is already used by other player
            // usernames are unique per realm.

            // on success avatar/hero creation - save the avatar/hero information in DataManager.
            // load heroSelectionScene and let the user enter the realm with his new hero.

            if (Common.InputIsValid(usernameInputField, userNameRegexPattern))
            {
                createBtn.interactable = false;
                usernameInputField.interactable = false;
                errorLabel.gameObject.SetActive(false);
                FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

                Debug.Log("Valid username -> proceed with API request!");
                //TODO dont forget to set usernameInputField.interactable = true; after the request is complete

                string[] @params = new string[] { DataManager.Instance.CurrentRealmId.ToString(), DataManager.Instance.Id.ToString() };
                string endpoint = "realms/{0}/users/{1}/avatar";
                endpoint = string.Format(endpoint, @params);
                CreateHeroOrAvatarInput input = new CreateHeroOrAvatarInput
                {
                    heroClass = _selectedHeroClass,
                    heroFaction = _selectedFaction,
                    heroName = usernameInputField.text,
                };

                RequestManager.Instance.Post<CreateHeroOrAvatarInput>(endpoint, input, OnCreateHeroOrAvatarWithHero);
            }
            else
            {
                createBtn.interactable = true;
                usernameInputField.interactable = true;
                errorLabel.gameObject.SetActive(true);
                errorLabel.Find("Text").GetComponent<Text>().text = "Invalid username!";
            }
        }

        private void OnCreateHeroOrAvatarWithHero(HTTPRequest request, HTTPResponse response)
        {
            if (Common.RequestIsSuccessful(request, response))
            {
                Debug.Log("Request successful");
            }
        }

        private void OnFactionBtnPressed(FactionData faction, string factionName, Button factionBtn)
        {
            _selectedFaction = factionName;
            Common.HighlightButton(factionBtn, selectedFactionColor, factionNormalColor, _factionContainer);
            PopulateFactionInfoPanel(faction);
            InitializeClassButtons(factionName);
        }

        private void PopulateFactionInfoPanel(FactionData faction)
        {
            currentFactionIcon.sprite = faction.icon;
            currentFactionIcon.SetNativeSize();
            currentFactionTypeText.text = faction.name;
            currentFactionDescription.text = faction.description;
        }

        private void OnHeroClassBtnPressed(FactionData faction, string heroClass, Button classBtn)
        {
            _selectedHeroClass = heroClass;
            FactionClass @class = faction.heroes.FirstOrDefault(x => x.name == heroClass);
            currentClassIcon.sprite = @class.icon;
            currentClassIcon.SetNativeSize();
            currentClassTypeText.text = @class.name.ToUpper();
            currentClassDescription.text = @class.description;

            model3DText.text = @class.name.ToUpper();

            Common.HighlightButton(classBtn, selectedHeroColor, heroNormalColor, _classContainer);
        }

    }
}
