using UnityEngine;
using UnityEngine.SceneManagement;

namespace Strid {
    public class MenuHandler : MonoBehaviour {
        public void StartGame() {
            SceneManager.LoadSceneAsync("Scenes/GameScene", LoadSceneMode.Additive);
            SceneManager.UnloadSceneAsync("Scenes/MainMenu");
        }
    }
}