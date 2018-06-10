using Assets.Scripts.UI.MainMenu;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RealmSelectionModal : Modal<RealmSelectionModal>
    {
        public override void OnClosePressed()
        {
            MainMenuManager.Instance.ShowInitialButtons();
            LoginModal.Open();
        }
    }
}
