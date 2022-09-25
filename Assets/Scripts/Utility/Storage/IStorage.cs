using JetBrains.Annotations;
using System.Collections.Generic;

namespace Strid.Utility.Storage {
    /// <summary>Stores objects and supports basic CRUD</summary>
    /// <typeparam name="T">Type stored. Must have a distinct ID.</typeparam>
    public interface IStorage<T> where T : IStorable {
        /// <summary>Adds a new item</summary>
        /// <param name="newItem">The added item, which cannot be null</param>
        void Insert(T newItem);

        /// <summary>Searches for an item by its ID</summary>
        /// <param name="id">ID of the object</param>
        /// <returns>The object if found, null otherwise</returns>
        [CanBeNull] T Find(int id);

        /// <returns>A list of all stored items, can be empty but not null</returns>
        [NotNull] List<T> FindAll();

        /// <returns>The number of items stored</returns>
        int Count();

        /// <summary>Changes the value of an item</summary>
        /// <param name="id">ID of the item to modify</param>
        /// <param name="updatedItem">The new value of the item</param>
        void Update(int id, T updatedItem);

        /// <summary>Removes an item</summary>
        /// <param name="id">ID of the item to delete</param>
        void Delete(int id);
    }
}