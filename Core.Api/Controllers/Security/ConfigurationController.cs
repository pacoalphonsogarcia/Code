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
    /// Represents a controller for exposing endpoints relating to Configurations
    /// </summary>
    public class ConfigurationController : BaseController<Configuration>  
	{
		public ConfigurationController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get Configuration")]
        public override Configuration Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query Configuration")]
        public override IEnumerable<Configuration> Get() => base.Get();

        [RequiresPermission("Create Configuration")]
        public override ICollection<string> Post(ICollection<Configuration> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update Configuration")]
        public override Configuration Put(string id, Configuration entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete Configuration")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



