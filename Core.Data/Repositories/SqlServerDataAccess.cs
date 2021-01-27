using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Data.Contracts;
using Core.Data.Models.Base;
using Microsoft.EntityFrameworkCore;

namespace Core.Data.Repositories
{
    /// <summary>
    /// Provides methods for data access to and from the data store
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public class SqlServerDataAccess<T> : IDataAccess<T> where T : BaseEntity
    {
        private DbSet<T> DatabaseSet { get; set; }
        private DbContext DataContext { get; set; }
        public SqlServerDataAccess(DbContext context)
        {
            DataContext = context;
            DatabaseSet = DataContext.Set<T>();
        }
        /// <summary>
        /// Adds a new entity to the data stored
        /// </summary>
        /// <param name="value">The entity to be added to the data store</param>
        /// <param name="commitToDataStore">If true, the method saves the entity before it goes out of scope; that is, it calls SaveChanges</param>
        /// <param name="generateId">If true, generates a new Id to be used as the entity's Id</param>
        /// <returns>The entity's Id</returns>
        public string Add(T value, bool commitToDataStore = true, bool generateId = true)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (generateId)
            {
                // The "N" parameter removes the dashes (hyphens) in the GUID
                value.Id = Guid.NewGuid().ToString("N");
            }
            value.LastUpdatedUtc = DateTime.UtcNow;
            value.Version = 1;
            var entityEntry = DataContext.Entry(value);
            if (entityEntry.State != EntityState.Detached)
            {
                entityEntry.State = EntityState.Added;
            }
            else
            {
                DatabaseSet.Add(value);
            }
            if (commitToDataStore)
            {
                DataContext.SaveChanges();
            }
            return value.Id;
        }
        /// <summary>
        /// Adds a set of new entities to the data store
        /// </summary>
        /// <param name="values">The set of entities to be added to the data store</param>
        /// <returns>The set of Ids of the saved entities</returns>
        public ICollection<string> AddRange(ICollection<T> values)
        {
            var returnCollection = new Collection<string>();
            foreach (var currentValue in values)
            {
                returnCollection.Add(Add(currentValue, false));
            }
            DataContext.SaveChanges();
            return returnCollection;
        }
        /// <summary>
        /// Either "soft deletes" or truly deletes an entity from the data store
        /// </summary>
        /// <param name="id">The entity Id</param>
        /// <param name="isHardDelete">If true, the entity is truly deleted from the database. Otherwise, only sets the "IsDeleted" field to true</param>
        public virtual void Delete(string id, bool isHardDelete = false)
        {
            var entityToDelete = Get(id);
            if (entityToDelete == null)
            {
                return;
            }
            var entityEntry = DataContext.Entry(entityToDelete);
            if (!isHardDelete)
            {
                // just update the entity and set the IsDeleted value to true
                entityToDelete.IsDeleted = true;
                Update(entityToDelete);
            }
            else
            {
                if (entityEntry != null && entityEntry.State != EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Deleted;
                }
                else
                {
                    DatabaseSet.Attach(entityToDelete);
                    DatabaseSet.Remove(entityToDelete);
                }
                DataContext.SaveChanges();
            }
        }
        /// <summary>
        /// Retrieves an entity based on the passed in Id
        /// </summary>
        /// <param name="id">The entity Id</param>
        /// <returns>The entity</returns>
        public T Get(string id)
        {
            return DatabaseSet.FirstOrDefault(p => p.Id == id);
        }
        /// <summary>
        /// Retrieves a set of entities
        /// </summary>
        /// <returns>The set of entities. This is lazy loaded; the records do not get loaded until a call to .ToList() or similar is performed</returns>
        public IQueryable<T> Get()
        {
            return DatabaseSet.AsQueryable();
        }
        /// <summary>
        /// Checks if the entity exists in the data store
        /// </summary>
        /// <param name="id">The Id of the entity to check</param>
        /// <returns>If true, the entity exists in the data store; otherwise, false</returns>
        public bool IsExisting(string id)
        {
            return DatabaseSet.Any(p => p.Id.ToLower() == id.ToLower());
        }
        /// <summary>
        /// Updates the current entity with the new value(s)
        /// </summary>
        /// <param name="value">The entity with the new values</param>
        /// <returns>The updated entity</returns>
        public T Update(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            value.LastUpdatedUtc = DateTime.UtcNow;
            value.Version = value.Version++;

            var returnUpdatedEntity = DataContext.Update(value);
            DataContext.SaveChanges();
            return returnUpdatedEntity.Entity;
        }
    }
}
