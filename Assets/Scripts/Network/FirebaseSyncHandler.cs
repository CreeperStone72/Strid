using System.Collections.Generic;
using UnityEngine;

namespace Strid.Network {
    using Gameplay.Cards;
    using Utility;
    
    public class FirebaseSyncHandler : MonoBehaviour {
        private FirebaseCardStorage _storage;

        public int count;
        
        [Space]
        
        public int searchId;
        public Card card;
        public List<Card> cards;
        
        private void Start() { _storage = new FirebaseCardStorage(this); }

        public void InsertCard() { _storage.Insert(cards[count++]); }

        public void InsertAll() {
            for (var index = 0; index < cards.Count; index++) {
                var c = cards[index];
                _storage.Insert(c, index);
            }
        }

        public void FindCard() { _storage.FindAsync(searchId, () => {
            var result = _storage.Find(searchId);
            if (result == null) Debug.Log("Null");
            else result.Log();
        }); }
        
        public void FindAllCards() { _storage.FindAllAsync(() => Loggable.LogAll(_storage.FindAll())); }

        public void UpdateCard() { _storage.Update(searchId, card); }
        
        public void DeleteCard() { _storage.Delete(searchId); }
    }
}