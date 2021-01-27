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
    /// Represents a controller for exposing endpoints relating to Apps
    /// </summary>
    public class AppController : BaseController<App>  
	{
        public AppController(CoreContext context) : base(context) { }

        [HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get App")]
        public override App Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query App")]
        public override IEnumerable<App> Get() => base.Get();


        [RequiresPermission("Create App")]
        public override ICollection<string> Post(ICollection<App> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update App")]
        public override App Put(string id, App entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete App")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



