using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Data.Contracts
{
    /// <summary>
    /// Provides method signatures for interfacing with a data store and most common actions
    /// </summary>
    public interface IDataAccess<T> where T : class
    {
        /// <summary>
        /// When implemented, retrieves an entity based on the passed in Id
        /// </summary>
        /// <param name="id">The entity Id</param>
        /// <returns>The entity</returns>
        T Get(string id);
        /// <summary>
        /// When implemented, retrieves a set of entities
        /// </summary>
        /// <returns>The set of entities. This is lazy loaded; the records do not get loaded until a call to .ToList() or similar is performed</returns>
        IQueryable<T> Get();
        /// <summary>
        /// When implemented, adds a new entity to the data store
        /// </summary>
        /// <param name="value">The entity to be added to the data store</param>
        /// <param name="commitToDataStore">If true, the method saves the entity before it goes out of scope; that is, it calls SaveChanges</param>
        /// <param name="generateId">If true, generates a new Id to be used as the entity's Id</param>
        /// <returns>The entity's Id</returns>
        string Add(T value, bool commitToDataStore = true, bool generateId = true);
        /// <summary>
        /// When implemented, updates the current entity with the new value(s)
        /// </summary>
        /// <param name="value">The entity with the new values</param>
        /// <returns>The updated entity</returns>
        T Update(T value);
        /// <summary>
        /// When implemented, either "soft deletes" or truly deletes an entity from the data store
        /// </summary>
        /// <param name="id">The entity Id</param>
        /// <param name="isHardDelete">If true, the entity is truly deleted from the database. Otherwise, only sets the "IsDeleted" field to true</param>
        void Delete(string id, bool isHardDelete = false);
        /// <summary>
        /// When implemented, checks if the entity exists in the data store
        /// </summary>
        /// <param name="id">The Id of the entity to check</param>
        /// <returns>If true, the entity exists in the data store; otherwise, false</returns>
        bool IsExisting(string id);
        /// <summary>
        /// When implemented, adds a set of new entities to the data store
        /// </summary>
        /// <param name="values">The set of entities to be added to the data store</param>
        /// <returns>The set of Ids of the saved entities</returns>
        ICollection<string> AddRange(ICollection<T> values);
    }
}