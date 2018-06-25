namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class SuccessRegistrationModal : Modal<SuccessRegistrationModal>
    {
        public override void OnClosePressed()
        {
            base.OnClosePressed();
            LoginModal.Instance.Open();
        }
    }
}
