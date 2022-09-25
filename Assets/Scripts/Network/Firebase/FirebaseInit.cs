using Firebase;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Strid.Network.Firebase {
    public class FirebaseInit : MonoBehaviour {
        public UnityEvent onFirebaseInitialized = new UnityEvent();

        private IEnumerator Start() {
            var task = FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => task.IsCompleted);
            
            if (task.Exception != null) Debug.LogError($"Failed to initialized Firebase: {task.Exception}");
            else onFirebaseInitialized.Invoke();
        }
    }
}