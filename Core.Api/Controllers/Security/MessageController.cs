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
    /// Represents a controller for exposing endpoints relating to Messages
    /// </summary>
    public class MessageController : BaseController<Message>  
	{
		public MessageController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get Message")]
        public override Message Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query Message")]
        public override IEnumerable<Message> Get() => base.Get();

        [RequiresPermission("Create Message")]
        public override ICollection<string> Post(ICollection<Message> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update Message")]
        public override Message Put(string id, Message entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete Message")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}