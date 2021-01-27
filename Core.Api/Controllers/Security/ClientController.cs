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
    /// Represents a controller for exposing endpoints relating to Clients
    /// </summary>
    public class ClientController : BaseController<Client>  
	{
        public ClientController(CoreContext context) : base(context) { }
		
        [HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get Client")]
        public override Client Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query Client")]
        public override IEnumerable<Client> Get() => base.Get();

        [RequiresPermission("Create Client")]
        public override ICollection<string> Post(ICollection<Client> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update Client")]
        public override Client Put(string id, Client entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete Client")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



