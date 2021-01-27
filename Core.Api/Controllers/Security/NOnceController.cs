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
    /// Represents a controller for exposing endpoints relating to NOnces
    /// </summary>
    public class NOnceController : BaseController<NOnce>  
	{
		public NOnceController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get NOnce")]
        public override NOnce Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query NOnce")]
        public override IEnumerable<NOnce> Get() => base.Get();

        [RequiresPermission("Create NOnce")]
        public override ICollection<string> Post(ICollection<NOnce> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update NOnce")]
        public override NOnce Put(string id, NOnce entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete NOnce")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



