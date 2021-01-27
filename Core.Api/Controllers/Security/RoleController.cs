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
    /// Represents a controller for exposing endpoints relating to Roles
    /// </summary>
    public class RoleController : BaseController<Role>  
	{
		public RoleController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get Role")]
        public override Role Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query Role")]
        public override IEnumerable<Role> Get() => base.Get();

        [RequiresPermission("Create Role")]
        public override ICollection<string> Post(ICollection<Role> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update Role")]
        public override Role Put(string id, Role entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete Role")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



