using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Data.Models;
using Assets.Scripts.UI.CharacterSelection;
using Assets.Scripts.UI.Modals;
using Assets.Scripts.UI.Modals.MainMenuModals;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.HeroCreation
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

                // delete request
                Debug.Log("Start Delete request");
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
