using Assets.Scripts.Data;
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
        public Button _realmBtnPrefab;

        // Loading image
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.linear;

        // Sorting
        public SortDirection sortDirection;
        public RealmsSortType sortType;

        private int _selectedRealmId;

        protected override void Start()
        {
            base.Start();
            ReloadRealms();

            _selectedRealmId = DataManager.Instance.CurrentRealmId;
            Debug.Log(_selectedRealmId);

            //TODO: Highlight the selected realm button

            // use _dataManager.CurrentRealmId to mark this realm as selected.
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

        private void ReloadRealms()
        {
            FormUtilities.Empty(_realmsContainer.transform);

            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

            string endpoint = "realms?orderBy={0}&pageSize={1}&orderDirection={2}";
            string[] @params = new string[] { sortType.ToString(), "50", sortDirection.ToString() };

            RequestManager.Instance.Get(endpoint, @params, OnGetRealmsRequestFinished);
        }

        private void OnGetRealmsRequestFinished(HTTPRequest request, HTTPResponse response)
        {
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
                realmButton.name = realm.id.ToString() + "_" + realm.name;
                realmButton.onClick.AddListener(delegate { OnRealmButtonPressed(realm.id); });

                TextMeshProUGUI btnText = realmButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();
                btnText.text = realm.name
                    + "<pos=25%>" + realm.avatarsCount
                    + "<pos=51%>" + realm.realmType
                    + "<pos=77.5%>" + realm.resetDate;
            }
        }

        private void OnRealmButtonPressed(int realmId)
        {
            _selectedRealmId = realmId;
            Debug.Log(_selectedRealmId);

            //TODO: Highlight the selected button
        }

        // build functionality to select realm by clicking on realm button
        // throw an UI error if no realm is selected and the user tries to press "Enter realm"

        // OnEnterRealmBtnPress ->
        // Do api call to check if the player has any heroes in this realm.
        // if not -> Open character creation screen
        // if true -> Open character selection screen.

        public override void OnClosePressed()
        {
            MainMenuManager.Instance.ShowInitialButtons();
            LoginModal.Open();
        }
    }
}
