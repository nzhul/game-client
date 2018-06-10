using UnityEngine;

namespace Assets.Scripts.UI.MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        private static MainMenuManager _instance;

        public static MainMenuManager Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void Start()
        {
            ShowInitialButtons();
        }

        public GameObject quitPanel;
        public GameObject smallButtonsPanel;

        public void HideInitialButtons()
        {
            quitPanel.SetActive(false);
            smallButtonsPanel.SetActive(false);
        }

        public void ShowInitialButtons()
        {
            quitPanel.SetActive(true);
            smallButtonsPanel.SetActive(true);
        }

        public void OnQuitButtonPressed()
        {
            Application.Quit();
        }
    }
}
