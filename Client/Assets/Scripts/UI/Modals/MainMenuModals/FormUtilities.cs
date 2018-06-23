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

        public static void DisplayErrorLabels(ErrorModel errorModel, IDictionary<string, InputField> formInputs)
        {
            foreach (var item in formInputs)
            {
                var oldLabel = item.Value.transform.Find("formErrorLabel");
                if (oldLabel != null)
                {
                    oldLabel.gameObject.SetActive(false);
                }
            }

            foreach (var property in errorModel.GetType().GetFields())
            {
                string pName = property.Name;
                string[] pValue = (string[])property.GetValue(errorModel);

                if (pValue != null && pValue.Length > 0)
                {
                    InputField field = formInputs[pName + "Field"];
                    Transform label = field.transform.Find("formErrorLabel");
                    label.gameObject.SetActive(true);
                    Text labelText = label.GetComponentInChildren<Text>();
                    labelText.text = string.Join("! ", pValue);
                }
            }
        }

        public static void Empty(Transform parent)
        {
            foreach (Transform child in parent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    public class FormData
    {
        public IDictionary<string, InputField> Inputs { get; set; }

        public ICollection<Button> Buttons { get; set; }
    }
}
