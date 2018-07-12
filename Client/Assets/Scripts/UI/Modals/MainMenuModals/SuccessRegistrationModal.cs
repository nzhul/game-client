namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class SuccessRegistrationModal : Modal<SuccessRegistrationModal>
    {
        public override void Close()
        {
            base.Close();
            LoginModal.Instance.Open();
        }
    }
}
