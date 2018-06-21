using Assets.Scripts.Network;
using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.UI.MainMenu;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RealmSelectionModal : Modal<RealmSelectionModal>
    {
        private const float buttonHeight = 55f;
        public RectTransform _realmsContainer;
        public Button _realmBtnPrefab;

        //Loading image
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.linear;

        protected override void Start()
        {
            base.Start();

            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

            string endpoint = "realms?orderBy={0}&pageSize={1}&orderDirection={2}";
            string[] @params = new string[] { "avatarsCount", "50", "descending"};

            RequestManager.Instance.Get(endpoint, @params, OnGetRealmsRequestFinished);

            // use _dataManager.CurrentRealmId to mark this realm as selected.
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
                Text btnText = realmButton.transform.Find("Text").GetComponent<Text>();
                btnText.text = realm.name + " -> " + realm.avatarsCount;
            }
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
