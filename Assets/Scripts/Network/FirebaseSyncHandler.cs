using UnityEngine;

namespace Strid.Network {
    using Gameplay;
    using Utility;
    
    public class FirebaseSyncHandler : MonoBehaviour {
        private FirebaseCardStorage _storage;
        public int searchId;
        public Card card;
        
        private void Start() { _storage = new FirebaseCardStorage(this); }

        public void InsertCard() { _storage.Insert(card); }

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