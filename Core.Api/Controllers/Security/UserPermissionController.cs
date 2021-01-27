using System.Collections.Generic;
using Core.Api.Controllers.Base;
using Core.Data.Attributes;
using Microsoft.AspNetCore.Mvc;
using Core.Data.Contexts;
using Core.Data.Models.Entities.Security;
using Microsoft.AspNet.OData;

namespace Core.Api.Controllers.Security
{
    /// <summary>
    /// Represents a controller for exposing endpoints relating to UserPermissions
    /// </summary>
    public class UserPermissionController : BaseController<UserPermission>  
	{
        public UserPermissionController(CoreContext context) : base(context) { }

        [HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get UserPermission")]
        public override UserPermission Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query UserPermission")]
        public override IEnumerable<UserPermission> Get() => base.Get();

        [RequiresPermission("Create UserPermission")]
        public override ICollection<string> Post(ICollection<UserPermission> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update UserPermission")]
        public override UserPermission Put(string id, UserPermission entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete UserPermission")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



