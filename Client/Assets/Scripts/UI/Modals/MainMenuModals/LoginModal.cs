﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Data;
using Assets.Scripts.Network.RequestModels.Users.Input;
using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.Network.Services;
using Assets.Scripts.Network.Shared.Http;
using Assets.Scripts.UI.MainMenu;
using BestHTTP;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class LoginModal : Modal<LoginModal>
    {
        private const string USERNAME_KEY = "UsernameField";
        private const string PASSWORD_KEY = "PasswordField";

        private DataManager _dataManager;
        private LoginInput _loginModel;
        private IUserService _userService;

        // form fields
        protected IDictionary<string, InputField> formInputs;
        protected ICollection<Button> formButtons;
        public GameObject _form;
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.easeInOutExpo;
        public GameObject errorMessagePanel;
        public Toggle rememberMeToggle;
        private Text errorMessageText;

        protected override void Start()
        {
            FormData formData = FormUtilities.GenerateFormData(_form);
            formButtons = formData.Buttons;
            formInputs = formData.Inputs;
            errorMessageText = errorMessagePanel.GetComponentInChildren<Text>();

            _dataManager = DataManager.Instance;
            _userService = new UserService();

            if (!string.IsNullOrEmpty(_dataManager.Token))
            {
                if (_dataManager.RememberMe
                    && !string.IsNullOrEmpty(_dataManager.Username)
                    && !string.IsNullOrEmpty(_dataManager.Password))
                {
                    PopulateFormFields();
                }
            }
        }

        private void PopulateFormFields()
        {
            formInputs[FormUtilities.USERNAME_KEY].text = _dataManager.Username;
            formInputs[FormUtilities.PASSWORD_KEY].text = _dataManager.Password;
            rememberMeToggle.isOn = _dataManager.RememberMe;
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
            _loginModel = loginModel;

            // TODO: validate loginModel - Server side or client side

            RequestManager.Instance.Post<LoginInput>("auth/login", loginModel, OnRequestFinished);
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
                else if (request != null && request.Exception != null)
                {
                    if (request.Exception.Message.Contains("No connection could be made"))
                    {
                        errorMessage = "Please check your internet connection!";
                    }
                }

                errorMessageText.text = errorMessage;
                errorMessagePanel.SetActive(true);
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
                //LoginResponse loginInfo = JsonUtility.FromJson<LoginResponse>(json);
                LoginResponse loginInfo = JsonConvert.DeserializeObject<LoginResponse>(json);

                if (loginInfo != null)
                {
                    StoreUserData(loginInfo);
                }

                _userService.SendAuthRequest(loginInfo.user.id, loginInfo.user.username, loginInfo.tokenString);

                MainMenuManager.Instance.HideInitialButtons();
                RealmSelectionModal.Instance.Open();
                errorMessagePanel.SetActive(false);
            }

            FormUtilities.HideLoadingIndicator(_loadingImage);
            FormUtilities.DisableForm(false, formInputs, formButtons);
        }

        private void StoreUserData(LoginResponse loginInfo)
        {
            if (rememberMeToggle != null)
            {
                _dataManager.RememberMe = rememberMeToggle.isOn;

                if (_dataManager.RememberMe == true)
                {
                    _dataManager.Password = _loginModel.password;
                }
            }

            _dataManager.Token = loginInfo.tokenString;
            _dataManager.Id = loginInfo.user.id;
            _dataManager.Username = loginInfo.user.username;
            _dataManager.Created = loginInfo.user.dateCreated.Value;
            _dataManager.LastActive = loginInfo.user.dateLastActive.Value;
            _dataManager.Gender = loginInfo.user.gender;
            _dataManager.Age = loginInfo.user.age;
            _dataManager.CurrentRealmId = loginInfo.user.currentRealmId;
            _dataManager.Save();
        }
    }
}
