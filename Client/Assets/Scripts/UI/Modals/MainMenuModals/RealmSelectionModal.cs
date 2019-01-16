using System;
using Assets.Scripts.Data;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Network.RequestModels.Sorting;
using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.Shared.DataModels;
using Assets.Scripts.UI.MainMenu;
using Assets.Scripts.Utilities;
using BestHTTP;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RealmSelectionModal : Modal<RealmSelectionModal>
    {
        private const float buttonHeight = 55f;
        public RectTransform _realmsContainer;
        public Scrollbar _realmsScrollbar;
        public Button _realmBtnPrefab;

        // Loading image
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.linear;

        // Sorting
        public SortDirection sortDirection;
        public RealmsSortType sortType;

        // Selected realm
        private int _selectedRealmId;
        public Color selectedRealmColor;
        public Color realmNormalColor;
        public RealmListItem[] _realmsList;

        public override void Open()
        {
            base.Open();
            _selectedRealmId = DataManager.Instance.CurrentRealmId;
            ReloadRealms();
        }

        public override void Close()
        {
            _selectedRealmId = 0;
            MainMenuManager.Instance.ShowInitialButtons();
            LoginModal.Instance.Open();
        }

        public void OnNameSortBtnPressed()
        {
            sortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            sortType = RealmsSortType.Name;
            ReloadRealms();
        }

        public void OnPopulationSortBtnPressed()
        {
            sortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            sortType = RealmsSortType.Population;
            ReloadRealms();
        }

        public void OnTypeSortBtnPressed()
        {
            sortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            sortType = RealmsSortType.Type;
            ReloadRealms();
        }

        public void OnResetDateSortBtnPressed()
        {
            sortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
            sortType = RealmsSortType.ResetDate;
            ReloadRealms();
        }

        //TODO: Loading animation on requests ?
        public void OnSelectRealmBtnPressed()
        {
            DataManager.Instance.CurrentRealmId = _selectedRealmId;

            string endpoint = "realms/{0}/users/{1}/updateCurrentRealm";
            string[] @params = new string[] { DataManager.Instance.CurrentRealmId.ToString(), DataManager.Instance.Id.ToString() };

            RequestManager.Instance.Put(endpoint, @params, DataManager.Instance.Token, OnUpdateCurrentRealmRequestFinished);

            string avatarEndpoint = "realms/{0}/users/{1}/avatar";
            RequestManager.Instance.Get(avatarEndpoint, @params, DataManager.Instance.Token, OnGetUserAvatarRequestFinished);
        }

        private void OnGetUserAvatarRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            string errorMessage;
            if (NetworkCommon.RequestIsSuccessful(request, response, out errorMessage))
            {
                string json = response.DataAsText;
                UserAvatar userAvatar = JsonConvert.DeserializeObject<UserAvatar>(json);

                if (userAvatar != null)
                {
                    DataManager.Instance.Avatar = userAvatar;
                    DataManager.Instance.Save();

                    if (userAvatar.heroes != null && userAvatar.heroes.Count > 0)
                    {
                        //TODO: Transitions
                        LevelLoader.LoadLevel(LevelLoader.HERO_SELECTION_SCENE);
                    }
                    else
                    {
                        //TODO: Transitions
                        LevelLoader.LoadLevel(LevelLoader.HERO_CREATION_SCENE);
                    }
                }
                else
                {
                    // user is logging for the first time in this realm
                    // in CharacterCreationMenuScene - check if DataManager.Instance.Avatar == null
                    // if true - create new avatar for the user.

                    LevelLoader.LoadLevel(LevelLoader.HERO_CREATION_SCENE);
                }
            }

        }

        private void ReloadRealms()
        {
            Common.Empty(_realmsContainer.transform);

            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

            string endpoint = "realms?orderBy={0}&pageSize={1}&orderDirection={2}";
            string[] @params = new string[] { sortType.ToString(), "50", sortDirection.ToString() };

            RequestManager.Instance.Get(endpoint, @params, DataManager.Instance.Token, OnGetRealmsRequestFinished);
        }

        private void OnUpdateCurrentRealmRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            //TODO: Handle with the global request handler
        }

        private void OnGetRealmsRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            string errorMessage;
            if (NetworkCommon.RequestIsSuccessful(request, response, out errorMessage))
            {
                string json = response.DataAsText;
                RealmListItem[] realms = JsonConvert.DeserializeObject<RealmListItem[]>(json);

                if (realms != null)
                {
                    _realmsList = realms;
                    InitializeRealmButtons(realms);
                }
            }

            FormUtilities.HideLoadingIndicator(_loadingImage);
        }

        private void InitializeRealmButtons(RealmListItem[] realms)
        {
            _realmsContainer.sizeDelta = new Vector2(_realmsContainer.sizeDelta.x, realms.Length * buttonHeight);

            foreach (RealmListItem realm in realms)
            {
                Button realmButton = GameObject.Instantiate<Button>(_realmBtnPrefab, _realmsContainer.transform);
                realmButton.name = realm.id.ToString() + "_RealmBtn";
                realmButton.onClick.AddListener(delegate { OnRealmButtonPressed(realm.id); });

                TextMeshProUGUI btnText = realmButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                btnText.text = realm.name
                    + "<pos=25%>" + realm.avatarsCount
                    + "<pos=51.5%>" + realm.realmType
                    + "<pos=77.5%>" + realm.resetDate;
            }

            if (realms != null && realms.Length > 0 && _selectedRealmId != 0 && _realmsContainer != null)
            {
                HighlightSelectedRealm(true);
            }
        }

        private void OnRealmButtonPressed(int realmId)
        {
            _selectedRealmId = realmId;
            HighlightSelectedRealm(false);
        }

        private void HighlightSelectedRealm(bool updateScrollbar)
        {
            //TODO: Foreach all buttons and set normal color to default;

            foreach (Transform btnObject in _realmsContainer.transform)
            {
                Button btn = btnObject.GetComponent<Button>();
                ColorBlock cb1 = btn.colors;
                cb1.normalColor = realmNormalColor;
                btn.colors = cb1;
            }

            int realmIndex = Array.FindIndex(_realmsList, r => r.id == _selectedRealmId);

            Button selectedRealmBtn = _realmsContainer.Find(_selectedRealmId.ToString() + "_RealmBtn").GetComponent<Button>();
            ColorBlock cb = selectedRealmBtn.colors;
            cb.normalColor = selectedRealmColor;
            selectedRealmBtn.colors = cb;

            if (_realmsScrollbar != null && updateScrollbar)
            {
                _realmsScrollbar.value = CalculateScrollbarValue(_realmsList.Length, realmIndex);
            }
        }

        private float CalculateScrollbarValue(int realmsCount, int realmIndex)
        {
            float middlePoint = realmsCount / (float)2;
            float scrollValue = 1 - (realmIndex * (0.5f / middlePoint));

            if (scrollValue < 0.2f)
            {
                scrollValue = 0;
            }

            if (scrollValue > 0.8f)
            {
                scrollValue = 1;
            }

            return scrollValue;
        }

        // build functionality to select realm by clicking on realm button
        // throw an UI error if no realm is selected and the user tries to press "Enter realm"

        // OnEnterRealmBtnPress ->
        // Do api call to check if the player has any heroes in this realm.
        // if not -> Open character creation screen
        // if true -> Open character selection screen.
    }
}
