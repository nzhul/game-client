using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core.Network;
using Assets.Scripts.RequestModels.Users.Input;
using Assets.Scripts.RequestModels.Users.View;
using Assets.Scripts.UI.MainMenu;
using BestHTTP;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class LoginModal : Modal<LoginModal>
    {
        private const string USERNAME_KEY = "UsernameField";
        private const string PASSWORD_KEY = "PasswordField";

        public GameObject _form;
        public GameObject _loadingImage;

        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.easeInOutExpo;

        public Text errorMessageText;

        private IDictionary<string, InputField> formInputs;
        private ICollection<Button> formButtons;

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            formInputs = new Dictionary<string, InputField>();

            ICollection<InputField> fieldElements = _form.GetComponentsInChildren<InputField>();
            foreach (var field in fieldElements)
            {
                if (!formInputs.ContainsKey(field.name))
                {
                    formInputs.Add(field.name, field);
                }
            }

            formButtons = _form.GetComponentsInChildren<Button>();
        }

        public void OnLoginPressed()
        {
            StartCoroutine(OnLoginPressedRoutine());
        }

        private IEnumerator OnLoginPressedRoutine()
        {
            yield return new WaitForSeconds(0);

            DisableForm(true);

            ShowLoadingIndicator();

            LoginInput loginModel = GenerateInputData();

            // TODO: validate loginModel

            RequestManager.Instance.Post<LoginInput>("/api/auth/login", loginModel, OnRequestFinished);
        }

        private LoginInput GenerateInputData()
        {
            string username = formInputs[USERNAME_KEY].text;
            string password = formInputs[PASSWORD_KEY].text;

            return new LoginInput
            {
                username = username,
                password = password
            };
        }

        private void OnRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (response == null || response.StatusCode != 200)
            {
                Debug.LogWarning("Login request failed");

                string errorMessage = "Server error";

                if (response != null)
                {
                    switch (response.StatusCode)
                    {
                        case 401:
                            errorMessage = "Invalid username or password";
                            break;
                        default:
                            errorMessage = "Server error";
                            break;
                    }
                }

                errorMessageText.text = errorMessage;
                errorMessageText.gameObject.SetActive(true);
            }
            else
            {
                // on success:
                // store player token
                // play transition
                // Load next level 
                // yield return delay
                //yield return new WaitForSeconds(_playDelay);


                string json = response.DataAsText;
                LoginResponse loginInfo = JsonUtility.FromJson<LoginResponse>(json);

                MainMenuManager.Instance.HideInitialButtons();
                RealmSelectionModal.Open();
                errorMessageText.gameObject.SetActive(false);
            }

            HideLoadingIndicator();
            DisableForm(false);
        }

        private void ShowLoadingIndicator()
        {
            _loadingImage.SetActive(true);
            iTween.RotateBy(_loadingImage, iTween.Hash(
                    "z", loadingImageSpeed,
                    "easetype", loadingImageEaseType,
                    "looptype", iTween.LoopType.loop
                ));
        }

        private void HideLoadingIndicator()
        {
            iTween.Stop(_loadingImage);
            _loadingImage.SetActive(false);
        }

        private void DisableForm(bool disabled)
        {
            foreach (var field in formInputs)
            {
                field.Value.interactable = !disabled;
            }

            foreach (var button in formButtons)
            {
                button.interactable = !disabled;
            }
        }
    }
}
