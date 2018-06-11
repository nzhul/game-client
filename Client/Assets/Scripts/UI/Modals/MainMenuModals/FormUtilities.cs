using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class FormUtilities
    {
        public const string USERNAME_KEY = "UsernameField";
        public const string EMAIL_KEY = "EmailField";
        public const string PASSWORD_KEY = "PasswordField";
        public const string CONFIRM_PASSWORD_KEY = "ConfirmPasswordField";

        public static FormData GenerateFormData(GameObject form)
        {
            IDictionary<string, InputField> formInputs = new Dictionary<string, InputField>();

            ICollection<InputField> fieldElements = form.GetComponentsInChildren<InputField>();
            foreach (var field in fieldElements)
            {
                if (!formInputs.ContainsKey(field.name))
                {
                    formInputs.Add(field.name, field);
                }
            }

            ICollection<Button> formButtons = form.GetComponentsInChildren<Button>();

            return new FormData
            {
                Inputs = formInputs,
                Buttons = formButtons
            };
        }

        public static void ShowLoadingIndicator(GameObject _loadingImage, float loadingImageSpeed, iTween.EaseType loadingImageEaseType)
        {
            _loadingImage.SetActive(true);
            iTween.RotateBy(_loadingImage, iTween.Hash(
                    "z", loadingImageSpeed,
                    "easetype", loadingImageEaseType,
                    "looptype", iTween.LoopType.loop
                ));
        }

        public static void HideLoadingIndicator(GameObject _loadingImage)
        {
            iTween.Stop(_loadingImage);
            _loadingImage.SetActive(false);
        }

        public static void DisableForm(bool disabled, IDictionary<string, InputField> formInputs, ICollection<Button> formButtons)
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

        public static void DisplayErrorLabels(ErrorModel errorModel, IDictionary<string, InputField> formInputs, GameObject errorLabelPrefab)
        {
            foreach (var item in formInputs)
            {
                var oldLabel = item.Value.transform.Find("formErrorLabel(Clone)");
                if (oldLabel != null)
                {
                    GameObject.DestroyImmediate(oldLabel.gameObject);
                }
            }

            // TODO: This shift with 65f is causing problems on different resolutions.
            // consider just hiding the error messages and display them when needed instead of render them out of nothing every time.
            // this will probably solve the issue
            // TODO: Refactor this brutal copy paste

            if (errorModel.Username != null && errorModel.Username.Length > 0)
            {
                InputField field = formInputs[USERNAME_KEY];
                GameObject label = GameObject.Instantiate(errorLabelPrefab, 
                    new Vector3(field.transform.position.x, field.transform.position.y + 65f, field.transform.position.z), 
                    Quaternion.identity, field.transform);
                Text labelText = label.GetComponentInChildren<Text>();
                labelText.text = string.Join("! ", errorModel.Username);
            }

            if (errorModel.Email != null && errorModel.Email.Length > 0)
            {
                InputField field = formInputs[EMAIL_KEY];
                GameObject label = GameObject.Instantiate(errorLabelPrefab,
                    new Vector3(field.transform.position.x, field.transform.position.y + 65f, field.transform.position.z),
                    Quaternion.identity, field.transform);
                Text labelText = label.GetComponentInChildren<Text>();
                labelText.text = string.Join("! ", errorModel.Email);
            }

            if (errorModel.Password != null && errorModel.Password.Length > 0)
            {
                InputField field = formInputs[PASSWORD_KEY];
                GameObject label = GameObject.Instantiate(errorLabelPrefab,
                    new Vector3(field.transform.position.x, field.transform.position.y + 65f, field.transform.position.z),
                    Quaternion.identity, field.transform);
                Text labelText = label.GetComponentInChildren<Text>();
                labelText.text = string.Join("! ", errorModel.Password);
            }

            if (errorModel.ConfirmPassword != null && errorModel.ConfirmPassword.Length > 0)
            {
                InputField field = formInputs[CONFIRM_PASSWORD_KEY];
                GameObject label = GameObject.Instantiate(errorLabelPrefab,
                    new Vector3(field.transform.position.x, field.transform.position.y + 65f, field.transform.position.z),
                    Quaternion.identity, field.transform);
                Text labelText = label.GetComponentInChildren<Text>();
                labelText.text = string.Join("! ", errorModel.ConfirmPassword);
            }
        }
    }

    public class FormData
    {
        public IDictionary<string, InputField> Inputs { get; set; }

        public ICollection<Button> Buttons { get; set; }
    }
}
