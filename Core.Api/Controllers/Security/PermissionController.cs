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
    /// Represents a controller for exposing endpoints relating to Permissions
    /// </summary>
    public class PermissionController : BaseController<Permission>  
	{
        public PermissionController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get Permission")]
        public override Permission Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query Permission")]
        public override IEnumerable<Permission> Get() => base.Get();

        [RequiresPermission("Create Permission")]
        public override ICollection<string> Post(ICollection<Permission> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update Permission")]
        public override Permission Put(string id, Permission entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete Permission")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}