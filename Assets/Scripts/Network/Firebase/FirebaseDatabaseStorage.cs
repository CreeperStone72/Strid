using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Strid.Network.Firebase {
    using Utility.Storage;
    
    public abstract class FirebaseDatabaseStorage<T> : IStorage<T> where T : IStorable {
        private readonly DatabaseReference _ref;

        private int _count;
        private T _searchResult;
        private readonly List<T> _items;

        protected FirebaseDatabaseStorage(string tableName) {
            _ref = FirebaseDatabase.DefaultInstance.RootReference.Child(tableName);

            _count = 0;
            _searchResult = default;
            _items = new List<T>();
        }
        
        #region Abstract methods
        
        protected abstract string ToJson(T obj);
        protected abstract Task<T> FromJson(string json);
        
        #endregion

        #region CRUD overrides

        public void Insert(T newItem) { Insert(newItem, -1); }
        
        /// <summary>Inserts a new item in the database</summary>
        /// <param name="newItem">Item to insert</param>
        /// <param name="id">(Optional) Id of the item</param>
        public void Insert(T newItem, int id) {
            CountUpdate(() => {
                var itemId = id == -1 ? _count : id;
                newItem.SetId(itemId);
                GetChild(itemId).SetRawJsonValueAsync(ToJson(newItem)).ContinueWith(it => _ref.Child("count").SetValueAsync(++_count));
            });
        }

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
        public void Update(int id, T updatedItem) { GetChild(id).SetRawJsonValueAsync(ToJson(updatedItem)); }

        /// <summary>Removes an item from the database</summary>
        /// <param name="id">ID of the object to delete</param>
        public void Delete(int id) { GetChild(id).RemoveValueAsync(); }

        #endregion

        #region Async CRUD
        
        /// <summary>Searches for an item in the database<br />NOTE: Once found, you can access the item with Find()</summary>
        /// <param name="id">ID of the object to find</param>
        /// <param name="callback">What to do once the item has been found</param>
        public void FindAsync(int id, Action callback) {
            GetChild(id).GetValueAsync().ContinueWithOnMainThread(findTask => {
                FromJson(findTask.Result.GetRawJsonValue()).ContinueWithOnMainThread(jsonTask => {
                    _searchResult = findTask.Result == null ? default : jsonTask.Result;
                    callback.Invoke();
                });
            });
        }
        
        /// <summary>Searches for all stored items<br />NOTE: Once found, you can access the items with FindAll()</summary>
        /// <param name="callback">What to do once the items has been found</param>
        public void FindAllAsync(Action callback) {
            _ref.OrderByKey().GetValueAsync().ContinueWithOnMainThread(async findAllTask => {
                _items.Clear();
                
                if (findAllTask.Result != null) {
                    var children = findAllTask.Result.Children.ToList();
                    
                    foreach (var child in children.Take(children.Count - 1)) {
                        try {
                            var jsonTask = FromJson(child.GetRawJsonValue());
                            await jsonTask;
                            _items.Add(jsonTask.Result);
                        } catch (Exception e) { Debug.LogException(e); }
                    }
                }
            
                callback.Invoke();
            });
        }

        private void CountUpdate(Action callback) {
            _ref.Child("count").GetValueAsync().ContinueWithOnMainThread(countTask => {
                _count = Convert.ToInt32(countTask.Result.Value);
                callback.Invoke();
            });
        }

        #endregion
        
        private DatabaseReference GetChild(int id) { return _ref.Child(id.ToString()); }
    }
}