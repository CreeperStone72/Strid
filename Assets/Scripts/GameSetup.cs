using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Strid {
    using Gameplay.Cards;
    using Network;
    using Network.Firebase;
    
    [RequireComponent(typeof(FirebaseInit))]
    public class GameSetup : MonoBehaviour {
        private FirebaseCardStorage _storage;
        
        private void Start() { _storage = new FirebaseCardStorage(this); }

        public static List<Card> cards;

        /// <summary>Loads all the cards and opens the main menu</summary>
        public void OnStartGame() {
            _storage.FindAllAsync(() => {
                var allCardDeck = new Deck(_storage.FindAll());
                allCardDeck.Shuffle();
                
                cards = allCardDeck.Draw(54);
                
                SceneManager.LoadScene("Scenes/MainMenu", LoadSceneMode.Additive);
            });
        }
    }
}