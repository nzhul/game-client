using System;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.Network;
using Assets.Scripts.UI.CharacterSelection;
using Assets.Scripts.UI.Modals;
using Assets.Scripts.UI.Modals.MainMenuModals;
using Assets.Scripts.Utilities;
using BestHTTP;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.HeroSelection
{
    public class ConfirmDeleteModal : Modal<ConfirmDeleteModal>
    {
        private HeroSelectionManager _heroSelectionManager;
        public InputField confirmInput;
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.linear;
        public GameObject errorMessagePanel;
        private Text errorMessageText;
        public Button confirmDeleteBtn;

        protected override void Start()
        {
            base.Start();
            _heroSelectionManager = FindObjectOfType<HeroSelectionManager>();
            errorMessageText = errorMessagePanel.GetComponentInChildren<Text>();
        }

        public void OnConfirmDeleteBtnPressed()
        {
            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);
            confirmInput.interactable = false;
            confirmDeleteBtn.interactable = false;

            if (heroNamesAreEqual())
            {
                errorMessagePanel.SetActive(false);

                string endpoint = "realms/{0}/users/{1}/avatar/{2}/heroes/{3}";
                string[] @params = new string[]
                {
                    DataManager.Instance.CurrentRealmId.ToString(),
                    DataManager.Instance.Id.ToString(),
                    DataManager.Instance.Avatar.id.ToString(),
                    _heroSelectionManager.selectedHeroId.ToString()
                };

                RequestManager.Instance.Delete(endpoint, @params, OnDeleteRequestFinished);
            }
            else
            {
                // show validation error label

                errorMessageText.text = "Names do not match!";
                errorMessagePanel.SetActive(true);
                FormUtilities.HideLoadingIndicator(_loadingImage);
                confirmInput.interactable = true;
                confirmDeleteBtn.interactable = true;
            }
        }

        private void OnDeleteRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (Common.RequestIsSuccessful(request, response))
            {

            }
        }

        private bool heroNamesAreEqual()
        {
            int heroId = _heroSelectionManager.selectedHeroId;
            Hero selectedHero = DataManager.Instance.Avatar.heroes.FirstOrDefault(h => h.id == heroId);
            string confirmedHeroName = confirmInput.text;

            if (selectedHero.name == confirmedHeroName)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void OnOutsideClick()
        {
            base.OnClosePressed();
        }
    }
}
