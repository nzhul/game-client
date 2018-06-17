using Assets.Scripts.UI.MainMenu;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RealmSelectionModal : Modal<RealmSelectionModal>
    {
        protected override void Start()
        {
            // show loading image in the center of the panel
            // DO API call to get all availible realms
            // Build buttons for all realms and append them to the realms panel
            // hide the loading image
            // use _dataManager.CurrentRealmId to mark this realm as selected.
            base.Start();
        }

        // build functionality to select realm by clicking on realm button
        // throw an UI error if no realm is selected and the user tries to press "Enter realm"

        // OnEnterRealmBtnPress ->
        // Do api call to check if the player has any heroes in this realm.
        // if not -> Open character creation screen
        // if true -> Open character selection screen.

        public override void OnClosePressed()
        {
            MainMenuManager.Instance.ShowInitialButtons();
            LoginModal.Open();
        }
    }
}
