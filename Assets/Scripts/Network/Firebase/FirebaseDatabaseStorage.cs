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

        /// <summary>Inserts a new item in the database</summary>
        /// <param name="newItem">Item to insert</param>
        /// <param name="id">(Optional) Id of the item</param>
        public void Insert(T newItem) { Insert(newItem, -1); }
        public void Insert(T newItem, int id) { _parent.StartCoroutine(InsertAsync(newItem, id)); }

        /// <summary>Finds an object by its ID<br />NOTE: This only refers to the last search, use FindAsync() to actually search in the database</summary>
        /// <returns>The last search result</returns>
        public T Find(int id) { return _searchResult; }

        /// <summary>Finds all stored objects<br />NOTE: This only refers to the last search, use FindAllAsync() to actually search in the database</summary>
        /// <returns>The last search result</returns>
        public List<T> FindAll() { return _items; }

        /// <summary>Counts the stored items<br />NOTE: This only refers to the last search, use CountAsync() to actually search in the database</summary>
        /// <returns>The last count</returns>
        public int Count() { return _count; }

        /// <summary>Updates a given object</summary>
        /// <param name="id">ID of the object to update</param>
        /// <param name="updatedItem">New value of the item</param>
        public void Update(int id, T updatedItem) { _parent.StartCoroutine(UpdateAsync(id, updatedItem)); }

        /// <summary>Removes an item from the database</summary>
        /// <param name="id">ID of the object to delete</param>
        public void Delete(int id) { _parent.StartCoroutine(DeleteAsync(id)); }

        #endregion

        #region Async CRUD
        
        /// <summary>Searches for an item in the database<br />NOTE: Once found, you can access the item with Find()</summary>
        /// <param name="id">ID of the object to find</param>
        /// <param name="callback">What to do once the item has been found</param>
        public void FindAsync(int id, Action callback) { _parent.StartCoroutine(FindUpdate(id, callback)); }
        
        /// <summary>Searches for all stored items<br />NOTE: Once found, you can access the items with FindAll()</summary>
        /// <param name="callback">What to do once the items has been found</param>
        public void FindAllAsync(Action callback) { _parent.StartCoroutine(FindAllUpdate(callback)); }

        /// <summary>Counts the items in the database</summary>
        /// <returns>Number of items stored</returns>
        public async Task<int> CountAsync() {
            await CountUpdate();
            return Count();
        }

        #endregion
        
        #region Coroutines

        private IEnumerator InsertAsync(T newItem, int id = -1) {
            var task = CountUpdate();
            yield return new WaitUntil(() => task.IsCompleted);

            var itemId = id == -1 ? _count : id;
            
            newItem.SetId(itemId);

            var insertTask = GetChild(itemId).SetRawJsonValueAsync(JsonUtility.ToJson(newItem));
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