using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Strid {
    public class MenuHandler : MonoBehaviour {
        public GameObject menu;
        public GameObject loadingInterface;
        public Image loadingProgressBar;
        
        private readonly List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

        public void StartGame() {
            HideMenu();
            ShowLoadingScreen();
            _scenesToLoad.Add(SceneManager.LoadSceneAsync("Scenes/GameScene", LoadSceneMode.Additive));
            StartCoroutine(LoadingScreen());
            SceneManager.UnloadSceneAsync("Scenes/MainMenu");
        }

        private void HideMenu() { menu.SetActive(false); }

        private void ShowLoadingScreen() { loadingInterface.SetActive(true); }

        private IEnumerator LoadingScreen() {
            var totalProgress = 0f;

            foreach (var sceneLoad in _scenesToLoad) {
                totalProgress += sceneLoad.progress;
                loadingProgressBar.fillAmount = totalProgress / _scenesToLoad.Count;
                yield return null;
            }
        }
    }
}