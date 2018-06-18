using System;
using Assets.Scripts.Network.RequestModels.Users.View;
using Assets.Scripts.UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Modals.MainMenuModals
{
    public class RealmSelectionModal : Modal<RealmSelectionModal>
    {
        private const float buttonHeight = 55f;
        public RectTransform _realmsContainer;
        public Button _realmBtnPrefab;

        protected override void Start()
        {
            // show loading image in the center of the panel
            // DO API call to get all availible realms
            // Build buttons for all realms and append them to the realms panel
            // hide the loading image
            // use _dataManager.CurrentRealmId to mark this realm as selected.

            RealmListItem[] realms = new RealmListItem[]
            {
                new RealmListItem { id = 1, name = "Doom World", avatarsCount = 150, },
                new RealmListItem { id = 2, name = "Sandragosa", avatarsCount = 80, },
                new RealmListItem { id = 3, name = "Burning legion", avatarsCount = 65, },
                new RealmListItem { id = 4, name = "Land of confusion", avatarsCount = 71, },
                new RealmListItem { id = 5, name = "Noobs realm", avatarsCount = 20, },
                new RealmListItem { id = 1, name = "Doom World", avatarsCount = 150, },
                new RealmListItem { id = 2, name = "Sandragosa", avatarsCount = 80, },
                new RealmListItem { id = 3, name = "Burning legion", avatarsCount = 65, },
                new RealmListItem { id = 4, name = "Land of confusion", avatarsCount = 71, },
                new RealmListItem { id = 5, name = "Noobs realm", avatarsCount = 20, },
                new RealmListItem { id = 1, name = "Doom World", avatarsCount = 150, },
                new RealmListItem { id = 2, name = "Sandragosa", avatarsCount = 80, },
                new RealmListItem { id = 3, name = "Burning legion", avatarsCount = 65, },
                new RealmListItem { id = 4, name = "Land of confusion", avatarsCount = 71, },
                new RealmListItem { id = 5, name = "Noobs realm", avatarsCount = 20, },
            };

            InitializeRealmButtons(realms);
            base.Start();
        }

        private void InitializeRealmButtons(RealmListItem[] realms)
        {
            _realmsContainer.sizeDelta = new Vector2(_realmsContainer.sizeDelta.x, realms.Length * buttonHeight);

            for (int i = 0; i < realms.Length; i++)
            {
                Button realmButton = GameObject.Instantiate<Button>(_realmBtnPrefab, _realmsContainer.transform);
                Text btnText = realmButton.transform.Find("Text").GetComponent<Text>();
                btnText.text = realms[i].name + " -> " + realms[i].avatarsCount;
            }
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
