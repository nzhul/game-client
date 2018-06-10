﻿using System.Collections;
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

        // form fields
        protected IDictionary<string, InputField> formInputs;
        protected ICollection<Button> formButtons;
        public GameObject _form;
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.easeInOutExpo;
        public Text errorMessageText;

        protected override void Start()
        {
            FormData formData = FormUtilities.GenerateFormData(_form);
            formButtons = formData.Buttons;
            formInputs = formData.Inputs;
        }

        public void OnLoginPressed()
        {
            StartCoroutine(OnLoginPressedRoutine());
        }

        private IEnumerator OnLoginPressedRoutine()
        {
            yield return new WaitForSeconds(0);

            FormUtilities.DisableForm(true, formInputs, formButtons);

            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

            LoginInput loginModel = GenerateInputData();

            // TODO: validate loginModel - Server side or client side

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

            FormUtilities.HideLoadingIndicator(_loadingImage);
            FormUtilities.DisableForm(false, formInputs, formButtons);
        }
    }
}
