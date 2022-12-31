using System;
using System.Threading.Tasks;
using Firebase.Storage;
using UnityEngine;

namespace Strid.Network.Firebase {
    public class FirebaseImageStorage {
        // 4 bytes (1 per channel + alpha) for a 500x726 image
        private const long MaxAllowedSize = 4 * 500 * 726;

        private readonly StorageReference _reference;

        public FirebaseImageStorage(string tableName) { _reference = FirebaseStorage.DefaultInstance.RootReference.Child(tableName); }

        public void Upload(string name, Texture2D artwork) {
            GetChild(name).PutBytesAsync(artwork.GetRawTextureData()).ContinueWith(t => { if (HasFailed(t)) LogErr(t.Exception); });
        }

        public async Task<byte[]> Download(string name) {
            var bytes = Array.Empty<byte>();
            
            await _reference.Child(name).GetBytesAsync(MaxAllowedSize).ContinueWith(t => {
                if (HasFailed(t)) LogErr(t.Exception);
                else bytes = t.Result;
            });

            return bytes;
        }

        private StorageReference GetChild(string name) { return _reference.Child(name); }
        
        private static bool HasFailed(Task t) { return t.IsFaulted || t.IsCanceled; }
        private static void LogErr(Exception e) { Debug.LogException(e); }
    }
}