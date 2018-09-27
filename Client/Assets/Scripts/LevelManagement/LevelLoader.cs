using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.LevelManagement
{
    public static class LevelLoader
    {
        private static int mainMenuIndex = 0;

        public const string MAIN_MENU_SCENE = "01_MainMenu";
        public const string HERO_SELECTION_SCENE = "02_HeroSelectionMenu";
        public const string HERO_CREATION_SCENE = "03_HeroCreationMenu";
        public const string WORLD_SCENE = "04_World";

        public static void LoadLevel(string levelname)
        {
            if (Application.CanStreamedLevelBeLoaded(levelname))
            {
                SceneManager.LoadScene(levelname);
            }
            else
            {
                Debug.LogWarning("LEVELLOADER LoadLevel Error: invalid scene specified!");
            }
        }

        public static void LoadLevel(int levelIndex)
        {
            if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(levelIndex);
            }
            else
            {
                Debug.LogWarning("LEVELLOADER LoadLevel Error: invalid scene specified!");
            }
        }

        public static void ReloadLevel()
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            LoadLevel(currentLevelIndex);
        }

        public static void LoadNextLevel()
        {
            int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
            int nextLevelIndex = currentLevelIndex + 1;
            int totalSceneCount = SceneManager.sceneCountInBuildSettings;

            if (nextLevelIndex == totalSceneCount)
            {
                nextLevelIndex = mainMenuIndex;
            }

            LoadLevel(nextLevelIndex);

        }

        public static void LoadMainMenuLevel()
        {
            LoadLevel(mainMenuIndex);
        }
    }
}
