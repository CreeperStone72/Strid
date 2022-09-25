using UnityEngine;

namespace Strid.Network {
    using Firebase;
    using Gameplay;
    
    public class FirebaseCardStorage : FirebaseDatabaseStorage<Card> {
        private const string Table = "cards";

        public FirebaseCardStorage(MonoBehaviour parent) : base(Table, parent) { }
    }
}