using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Strid {
    using Gameplay.Cards;
    using Network;
    using Network.Firebase;
    
    [RequireComponent(typeof(FirebaseInit))]
    public class GameSetup : MonoBehaviour {
        [SerializeField] private Camera loadingCamera;
        
        private FirebaseCardStorage _storage;

        public static List<Card> cards;

        private void Start() {
            _storage = new FirebaseCardStorage();
            loadingCamera.gameObject.SetActive(true);
        }

        /// <summary>Loads all the cards and opens the main menu</summary>
        public void OnStartGame() {
            _storage.FindAllAsync(() => {
                Debug.Log(_storage.FindAll().Count(card => card != null));
                var allCardDeck = new Deck(_storage.FindAll());
                allCardDeck.Shuffle();
                
                cards = allCardDeck.Draw(54);

                loadingCamera.gameObject.SetActive(false);
                
                SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Additive);
            });
        }
    }
}