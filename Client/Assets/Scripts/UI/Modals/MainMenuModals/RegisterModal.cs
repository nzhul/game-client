using UnityEngine;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RegisterModal : Modal<RegisterModal>
    {
        public void OnBackToLoginPressed()
        {
            base.OnClosePressed();
            LoginModal.Open();
        }

        public void OnRegisterPressed()
        {
            Debug.Log("Register pressed!");
        }
    }
}