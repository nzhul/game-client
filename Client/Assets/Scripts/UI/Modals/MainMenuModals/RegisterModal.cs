using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Core.Network;
using Assets.Scripts.RequestModels.Users.Input;
using Assets.Scripts.RequestModels.Users.View;
using BestHTTP;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RegisterModal : Modal<RegisterModal>
    {

        // form fields
        protected IDictionary<string, InputField> formInputs;
        protected ICollection<Button> formButtons;
        public GameObject _form;
        public GameObject _loadingImage;
        public float loadingImageSpeed = 1f;
        public iTween.EaseType loadingImageEaseType = iTween.EaseType.easeInOutExpo;
        public GameObject errorMessagePanel;
        private Text errorMessageText;

        public void OnBackToLoginPressed()
        {
            base.OnClosePressed();
            LoginModal.Open();
        }

        protected override void Start()
        {
            FormData formData = FormUtilities.GenerateFormData(_form);
            formButtons = formData.Buttons;
            formInputs = formData.Inputs;
            errorMessageText = errorMessagePanel.GetComponentInChildren<Text>();
        }

        public void OnRegisterPressed()
        {
            StartCoroutine(OnRegisterPressedRoutine());
        }

        private IEnumerator OnRegisterPressedRoutine()
        {
            yield return new WaitForSeconds(0);

            FormUtilities.DisableForm(true, formInputs, formButtons);

            FormUtilities.ShowLoadingIndicator(_loadingImage, loadingImageSpeed, loadingImageEaseType);

            RegisterInput registerModel = GenerateInputData();

            // TODO: validate loginModel

            RequestManager.Instance.Post<RegisterInput>("api/auth/register", registerModel, OnRequestFinished);
        }

        private void OnRequestFinished(HTTPRequest request, HTTPResponse response)
        {
            if (response == null || response.StatusCode != 201)
            {
                Debug.LogWarning("Registration failed");

                string errorMessage = string.Empty;

                if (response != null)
                {
                    // Fail

                    switch (response.StatusCode)
                    {
                        case 400:
                            ErrorModel errorModel = ParseErrorModel(response.DataAsText); //TODO: handle different kind of errors.
                            if (errorModel != null)
                            {
                                //errorMessage = BuildServerErrorMessage(errorModel);
                                errorMessagePanel.SetActive(false);
                                FormUtilities.DisplayErrorLabels(errorModel, formInputs);
                            }
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

                    errorMessageText.text = errorMessage;
                    errorMessagePanel.SetActive(true);
                }
            }
            else
            {
                // Success
                // TODO: clear register form fields and clear all warning labels
                string json = response.DataAsText;
                RegistrationResponse registrationInfo = JsonUtility.FromJson<RegistrationResponse>(json);

                SuccessRegistrationModal.Open();
                errorMessagePanel.SetActive(false);
            }

            FormUtilities.HideLoadingIndicator(_loadingImage);
            FormUtilities.DisableForm(false, formInputs, formButtons);
        }

        //TODO: Delete this
        private string BuildServerErrorMessage(ErrorModel errorModel)
        {
            StringBuilder sb = new StringBuilder("Server error: ");

            if (errorModel.Username != null)
                sb.AppendLine(string.Join(", ", errorModel.Username));

            if (errorModel.Email != null)
                sb.AppendLine(string.Join(", ", errorModel.Email));

            if (errorModel.Password != null)
                sb.AppendLine(string.Join(", ", errorModel.Password));

            if (errorModel.ConfirmPassword != null)
                sb.AppendLine(string.Join(", ", errorModel.ConfirmPassword));

            return sb.ToString();
        }

        private ErrorModel ParseErrorModel(string dataAsText)
        {
            return JsonUtility.FromJson<ErrorModel>(dataAsText);
        }

        private RegisterInput GenerateInputData()
        {
            string username = formInputs[FormUtilities.USERNAME_KEY].text;
            string email= formInputs[FormUtilities.EMAIL_KEY].text;
            string password = formInputs[FormUtilities.PASSWORD_KEY].text;
            string confirmPassword = formInputs[FormUtilities.CONFIRM_PASSWORD_KEY].text;


            return new RegisterInput
            {
                username = username,
                email = email,
                password = password,
                confirmPassword = confirmPassword
            };
        }
    }


    public class ErrorModel
    {
        public string[] Username;

        public string[] Email;

        public string[] Password;

        public string[] ConfirmPassword;

    }
}