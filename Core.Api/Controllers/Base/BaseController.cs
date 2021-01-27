using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core.Exceptions;
using System.Net;
using System.Text;
using Core.Data.Contracts;
using Core.Data.Models.Base;
using Core.Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.Api.Controllers.Base
{
    /// <summary>
    /// A generic base controller with overridable methods with default behaviors.
    /// </summary>
    /// <typeparam name="T">The type of the entity. The entity must inherit from <see cref="BaseEntity"/></typeparam>
    [Route("api/[controller]")]
    public abstract class BaseController<T> : Controller where T : BaseEntity
    {
        private IDataAccess<T> DataAccess { get; }

        protected BaseController(DbContext context)
        {
            DataAccess = new SqlServerDataAccess<T>(context);
        }
        /// <summary>
        /// Gets a single entity based on the passed Id parameter
        /// </summary>
        /// <param name="id">The unique information identifying the entity</param>
        /// <returns>The entity</returns>
        [HttpGet("{id}")]
        public virtual T Get(string id)
        {
            var returnEntity = DataAccess.Get(id);
            return returnEntity;
        }
        /// <summary>
        /// Gets a collection of entities. This method is meant to be used in conjunction with OData queries.
        /// It is highly recommended to override this method as not doing will cause the entire collection to be returned,
        /// which may not be the desired behavior
        /// </summary>
        /// <returns>A collection of entities</returns>
        [HttpGet]
        public virtual IEnumerable<T> Get()
        {
            try
            {
                return DataAccess.Get();
            }
            catch (ParseException e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.Body.Write(Encoding.UTF8.GetBytes(e.Message), 0, e.Message.Length);
                throw;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.Body.Write(Encoding.UTF8.GetBytes(e.Message), 0, e.Message.Length);
                throw;
            }
        }
        /// <summary>
        /// Creates a collection of entities based on the values parameter
        /// </summary>
        /// <param name="values">The collection of entities to create</param>
        /// <returns>The collection of Ids of the newly created entities</returns>
        [HttpPost]
        public virtual ICollection<string> Post([FromBody] ICollection<T> values)
        {
            if (values != null && !values.Any())
            {
                const string errorMessage =
                    "Entity passed in was null. Be sure to pass in a value in the form of an array.";
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                Response.Body.Write(Encoding.UTF8.GetBytes(errorMessage), 0, errorMessage.Length);
                return null;
            }

            try
            {
                var returnStrings = DataAccess.AddRange(values);
                var returnResult = new List<string>();
                returnResult.AddRange(returnStrings);
                return returnResult;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.Body.Write(Encoding.UTF8.GetBytes(e.Message), 0, e.Message.Length);
                throw;
            }
        }
        /// <summary>
        /// Either updates the entity with the given Id parameter, or creates a new entity with its Id as the given Id parameter
        /// </summary>
        /// <param name="id">The unique information identifying the entity</param>
        /// <param name="value">The entity to be updated or to be created</param>
        /// <returns>The entity that was either updated or created</returns>
        [HttpPut("{id}")]
        public virtual T Put(string id, [FromBody] T value)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var entityExists = DataAccess.IsExisting(id);
            // if it's null, it must be a new record
            if (!entityExists)
            {
                value.Id = id;
                var returnResult = DataAccess.Add(value, true, false);
                var returnEntity = DataAccess.Get(returnResult);
                return returnEntity;
            }
            // If it isn't null, then it's an update
            else
            {
                DataAccess.Update(value);
                var returnEntity = DataAccess.Get(value.Id);
                return returnEntity;
            }
        }
        /// <summary>
        /// Deletes an entity from the data store based on the given Id parameter
        /// </summary>
        /// <param name="id">The unique information identifying the entity</param>
        [HttpDelete("{id}")]
        public virtual void Delete(string id)
        {
            DataAccess.Delete(id);
        }
    }
}