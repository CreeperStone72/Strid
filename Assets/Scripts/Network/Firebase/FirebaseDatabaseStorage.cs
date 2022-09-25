using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Strid.Network.Firebase {
    using Utility;
    using Utility.Storage;
    
    public abstract class FirebaseDatabaseStorage<T> : IStorage<T> where T : IStorable {
        private readonly DatabaseReference _reference;
        private readonly MonoBehaviour _parent;

        private int _count;
        private T _searchResult;
        private readonly List<T> _items;

        protected FirebaseDatabaseStorage(string tableName, MonoBehaviour parent) {
            _reference = FirebaseDatabase.DefaultInstance.RootReference.Child(tableName);
            _parent = parent;
            
            _count = 0;
            _searchResult = default;
            _items = new List<T>();
        }

        #region CRUD overrides

        public void Insert(T newItem) { _parent.StartCoroutine(InsertAsync(newItem)); }

        public T Find(int id) { return _searchResult; }

        public List<T> FindAll() { return _items; }

        public int Count() { return _count; }

        public void Update(int id, T updatedItem) { _parent.StartCoroutine(UpdateAsync(id, updatedItem)); }

        public void Delete(int id) { _parent.StartCoroutine(DeleteAsync(id)); }

        #endregion

        #region Async CRUD
        
        public void FindAsync(int id, Action callback) { _parent.StartCoroutine(FindUpdate(id, callback)); }

        public void FindAllAsync(Action callback) { _parent.StartCoroutine(FindAllUpdate(callback)); }

        public async Task<int> CountAsync() {
            await CountUpdate();
            return Count();
        }

        #endregion
        
        #region Coroutines

        private IEnumerator InsertAsync(T newItem) {
            var task = CountUpdate();
            yield return new WaitUntil(() => task.IsCompleted);
            
            newItem.SetId(_count);
            
            var insertTask = GetChild(_count).SetRawJsonValueAsync(JsonUtility.ToJson(newItem));
            yield return new WaitUntil(() => insertTask.IsCompleted);
            _reference.Child("count").SetValueAsync(++_count);
        }

        private IEnumerator FindUpdate(int id, Action callback) {
            var findTask = GetChild(id).GetValueAsync();
            yield return new WaitUntil(() => findTask.IsCompleted || findTask.IsFaulted);
            
            _searchResult = findTask.Result == null ? default : JsonUtility.FromJson<T>(findTask.Result.GetRawJsonValue());
            
            callback.Invoke();
        }

        private IEnumerator FindAllUpdate(Action callback) {
            var findAllTask = _reference.OrderByKey().GetValueAsync();
            yield return new WaitUntil(() => findAllTask.IsCompleted);
            
            _items.Clear();
            
            if (findAllTask.Result != null) {
                var children = findAllTask.Result.Children.ToList();
                foreach (var child in children.Take(children.Count - 1)) { _items.Add(JsonUtility.FromJson<T>(child.GetRawJsonValue())); }
            }
            
            callback.Invoke();
        }
        
        private async Task CountUpdate() {
            var countTask = _reference.Child("count").GetValueAsync();
            await TaskUtils.WaitUntil(() => countTask.IsCompleted);
            _count = (int) Convert.ToInt64(countTask.Result.Value);
        }

        private IEnumerator UpdateAsync(int id, T updatedItem) {
            var updateTask = GetChild(id).SetRawJsonValueAsync(JsonUtility.ToJson(updatedItem));
            yield return new WaitUntil(() => updateTask.IsCompleted);
        }
        
        private IEnumerator DeleteAsync(int id) {
            var deleteTask = GetChild(id).RemoveValueAsync();
            yield return new WaitUntil(() => deleteTask.IsCompleted);
        }

        #endregion

        private DatabaseReference GetChild(int id) { return _reference.Child(id.ToString()); }
    }
}