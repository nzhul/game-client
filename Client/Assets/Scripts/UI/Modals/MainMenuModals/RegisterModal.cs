using System.Collections;
using System.Collections.Generic;
using System.Text;
using Assets.Scripts.Network;
using Assets.Scripts.Network.RequestModels.Users.Input;
using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.Network.Shared.Http;
using BestHTTP;
using Newtonsoft.Json;
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
            base.Close();
            LoginModal.Instance.Open();
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

            RequestManager.Instance.Post<RegisterInput>("auth/register", registerModel, OnRequestFinished);
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
                                // TODO: display errors when errorModel.AltErrors != null and errorModel.AltErrors.Count > 0
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
                //RegistrationResponse registrationInfo = JsonUtility.FromJson<RegistrationResponse>(json);
                RegistrationResponse registrationInfo = JsonConvert.DeserializeObject<RegistrationResponse>(json);

                SuccessRegistrationModal.Instance.Open();
                errorMessagePanel.SetActive(false);
            }

            FormUtilities.HideLoadingIndicator(_loadingImage);
            FormUtilities.DisableForm(false, formInputs, formButtons);
        }

        //TODO: Delete this
        private string BuildServerErrorMessage(ErrorModel errorModel)
        {
            StringBuilder sb = new StringBuilder("Server error: ");

            if (errorModel.username != null)
                sb.AppendLine(string.Join(", ", errorModel.username));

            if (errorModel.email != null)
                sb.AppendLine(string.Join(", ", errorModel.email));

            if (errorModel.password != null)
                sb.AppendLine(string.Join(", ", errorModel.password));

            if (errorModel.confirmPassword != null)
                sb.AppendLine(string.Join(", ", errorModel.confirmPassword));

            return sb.ToString();
        }

        private ErrorModel ParseErrorModel(string dataAsText)
        {
            //return JsonUtility.FromJson<ErrorModel>(dataAsText);
            return JsonConvert.DeserializeObject<ErrorModel>(dataAsText);
        }

        private RegisterInput GenerateInputData()
        {
            string username = formInputs[FormUtilities.USERNAME_KEY].text;
            string email = formInputs[FormUtilities.EMAIL_KEY].text;
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
        public string[] username;

        public string[] email;

        public string[] password;

        public string[] confirmPassword;
    }
}