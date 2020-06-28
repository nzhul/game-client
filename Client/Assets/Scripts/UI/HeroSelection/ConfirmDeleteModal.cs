//using System.Linq;
//using Assets.Scripts.Data;
//using Assets.Scripts.LevelManagement;
//using Assets.Scripts.Network.Services;
//using Assets.Scripts.Shared.Models;
//using Assets.Scripts.UI.CharacterSelection;
//using Assets.Scripts.UI.Modals;
//using Assets.Scripts.UI.Modals.MainMenuModals;
//using BestHTTP;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Assets.Scripts.UI.HeroSelection
//{
//    public class ConfirmDeleteModal : Modal<ConfirmDeleteModal>
//    {
//        private HeroSelectionManager _heroSelectionManager;
//        public InputField confirmInput;
//        public GameObject _loadingImage;
//        public float loadingImageSpeed = 1f;
//        public iTween.EaseType loadingImageEaseType = iTween.EaseType.linear;
//        public GameObject errorMessagePanel;
//        private Text errorMessageText;
//        public Button confirmDeleteBtn;

//        protected override void Start()
//        {
//            base.Start();
//            _heroSelectionManager = FindObjectOfType<HeroSelectionManager>();
//            errorMessageText = errorMessagePanel.GetComponentInChildren<Text>();
//        }

//        public override void Open()
//        {
//            base.Open();
//            confirmInput.ActivateInputField();
//        }

//        public void OnConfirmDeleteBtnPressed()
//        {
//            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);
//            confirmInput.interactable = false;
//            confirmDeleteBtn.interactable = false;

//            if (heroNamesAreEqual())
//            {
//                errorMessagePanel.SetActive(false);

//                string endpoint = "realms/{0}/users/{1}/avatar/{2}/heroes/{3}";
//                string[] @params = new string[]
//                {
//                    DataManager.Instance.CurrentRealmId.ToString(),
//                    DataManager.Instance.Id.ToString(),
//                    DataManager.Instance.Avatar.Id.ToString(),
//                    _heroSelectionManager.selectedHeroId.ToString()
//                };

//                RequestManager.Instance.Delete(endpoint, @params, DataManager.Instance.Token, OnDeleteRequestFinished);
//            }
//            else
//            {
//                // show validation error label

//                errorMessageText.text = "Names do not match!";
//                errorMessagePanel.SetActive(true);
//                FormUtilities.HideLoadingIndicator(_loadingImage);
//                confirmInput.interactable = true;
//                confirmDeleteBtn.interactable = true;
//            }
//        }

//        private void OnDeleteRequestFinished(HTTPRequest request, HTTPResponse response)
//        {
//            string errorMessage;
//            if (NetworkCommon.RequestIsSuccessful(request, response, out errorMessage))
//            {
//                base.Close();
//                RemoveHero(_heroSelectionManager.selectedHeroId);
//                confirmInput.text = string.Empty;

//                if (DataManager.Instance.Avatar.Heroes.Count == 0)
//                {
//                    LevelLoader.LoadLevel(LevelLoader.HERO_CREATION_SCENE);
//                }
//                else
//                {
//                    _heroSelectionManager.InitializeHeroBtns();

//                    FormUtilities.HideLoadingIndicator(_loadingImage);
//                    confirmInput.interactable = true;
//                    confirmDeleteBtn.interactable = true;
//                }
//            }
//        }

//        private void RemoveHero(int heroId)
//        {
//            Hero heroToRemove = DataManager.Instance.Avatar.Heroes.FirstOrDefault(h => h.Id == heroId);
//            DataManager.Instance.Avatar.Heroes.Remove(heroToRemove);
//            DataManager.Instance.Save();
//        }

//        private bool heroNamesAreEqual()
//        {
//            int heroId = _heroSelectionManager.selectedHeroId;
//            Hero selectedHero = DataManager.Instance.Avatar.Heroes.FirstOrDefault(h => h.Id == heroId);
//            string confirmedHeroName = confirmInput.text;

//            if (selectedHero.Class.ToString() == confirmedHeroName)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public void OnOutsideClick()
//        {
//            base.Close();
//        }
//    }
//}
