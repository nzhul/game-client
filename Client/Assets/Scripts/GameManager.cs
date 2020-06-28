using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.LevelManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private string _currentScene;

        [SerializeField]
        private GameObject _loadingScreen;

        List<AsyncOperation> _loadingScenes = new List<AsyncOperation>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            _currentScene = LevelLoader.MAIN_MENU_SCENE;
            SceneManager.LoadSceneAsync(LevelLoader.MAIN_MENU_SCENE, LoadSceneMode.Additive);
        }

        public void LoadScene(string sceneName, bool displayLoadingScreen = true)
        {
            // TODO: Use Reflection to find Classes that implement IInitializer
            // When Loading a Scene, go through all IInitializer classes and check their IsDone and Progress properties ?

            if (displayLoadingScreen)
            {
                _loadingScreen.gameObject.SetActive(true);
            }

            _loadingScenes.Clear();
            _loadingScenes.Add(SceneManager.UnloadSceneAsync(_currentScene));
            _loadingScenes.Add(SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadingProgress());

            _currentScene = sceneName;
        }

        private IEnumerator GetSceneLoadingProgress()
        {
            foreach (var scene in _loadingScenes)
            {
                while (!scene.isDone)
                {
                    yield return null;
                }
            }

            // yield return new WaitForSeconds(5);

            _loadingScreen.gameObject.SetActive(false);
        }
    }
}
