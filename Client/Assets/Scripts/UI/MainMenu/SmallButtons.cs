using Assets.Scripts.UI.Modals.MainMenuModals;
using UnityEngine;

namespace Assets.Scripts.UI.MainMenu
{
    public class SmallButtons : MonoBehaviour
    {
        private const string WEBSITE_URL = "https://www.d3bg.org/index.php?group=1&cat=0";

        public void OnRegisterAccountPressed()
        {
            RegisterModal.Instance.Open();
        }

        public void OnOptionsPressed()
        {

        }

        public void OnTermsOfUsePressed()
        {

        }

        public void OnOfficialSitePressed()
        {
            Application.OpenURL(WEBSITE_URL);
        }
    }
}
