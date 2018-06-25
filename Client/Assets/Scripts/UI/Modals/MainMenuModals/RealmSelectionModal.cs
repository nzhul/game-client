using System;
using Assets.Scripts.Data;
using Assets.Scripts.LevelManagement;
using Assets.Scripts.Network;
using Assets.Scripts.Network.RequestModels.Sorting;
using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.UI.MainMenu;
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

        public override void OnClosePressed()
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

        public void OnSelectRealmBtnPressed()
        {
            DataManager.Instance.CurrentRealmId = _selectedRealmId;

            string endpoint = "realms/updateCurrentRealm/" + DataManager.Instance.Id + "/" + DataManager.Instance.CurrentRealmId;

            RequestManager.Instance.Put(endpoint, OnUpdateCurrentRealmRequestFinished);

            // FadeOut
            // Load Character selection scene with FadeIn

            // In the new scene onStart
            // Do api call to check if the player has any heroes in this realm.
            // if not -> Open character creation screen
            // if true -> Open character selection screen.

            Debug.Log("Loading next scene ...");

            DataManager.Instance.Save();

            LevelLoader.LoadNextLevel();
        }

        private void ReloadRealms()
        {
            FormUtilities.Empty(_realmsContainer.transform);

            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

            string endpoint = "realms?orderBy={0}&pageSize={1}&orderDirection={2}";
            string[] @params = new string[] { sortType.ToString(), "50", sortDirection.ToString() };

            RequestManager.Instance.Get(endpoint, @params, OnGetRealmsRequestFinished);
        }

        private void OnUpdateCurrentRealmRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            //TODO: Handle with the global request handler
            Debug.Log("Update request finished");
        }

        private void OnGetRealmsRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            //TODO: Handle with the global request handler

            if (response == null || response.StatusCode != 200)
            {
                Debug.LogWarning("Get realms request failed");

                string errorMessage = "Server error";

                if (response != null)
                {
                    switch (response.StatusCode)
                    {
                        case 401:
                            errorMessage = "Unauthorized";
                            break;
                        default:
                            errorMessage = "Server error";
                            break;
                    }
                }
                else if (request != null && request.Exception != null)
                {
                    if (request.Exception.Message.Contains("No connection could be made"))
                    {
                        errorMessage = "Please check your internet connection!";
                    }
                }

                Debug.LogWarning(errorMessage);

                // TODO: create global error message handler.
                // Something like popup that comes at the corner of the screen and disapears after a short period of time.
                // ref. -> alertify js

                //errorMessageText.text = errorMessage;
                //errorMessagePanel.SetActive(true);
            }
            else
            {
                // on success:

                string json = response.DataAsText;
                RealmListItem[] realms = JsonConvert.DeserializeObject<RealmListItem[]>(json);

                if (realms != null)
                {
                    _realmsList = realms;
                    InitializeRealmButtons(realms);
                }


                //errorMessagePanel.SetActive(false);
            }

            FormUtilities.HideLoadingIndicator(_loadingImage);
        }

        private void InitializeRealmButtons(RealmListItem[] realms)
        {
            _realmsContainer.sizeDelta = new Vector2(_realmsContainer.sizeDelta.x, realms.Length * buttonHeight);

            foreach (var realm in realms)
            {
                Button realmButton = GameObject.Instantiate<Button>(_realmBtnPrefab, _realmsContainer.transform);
                realmButton.name = realm.id.ToString() + "_RealmBtn";
                realmButton.onClick.AddListener(delegate { OnRealmButtonPressed(realm.id); });

                TextMeshProUGUI btnText = realmButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                btnText.text = realm.name
                    + "<pos=25%>" + realm.avatarsCount
                    + "<pos=51%>" + realm.realmType
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
            float middlePoint = (float)realmsCount / (float)2;
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
