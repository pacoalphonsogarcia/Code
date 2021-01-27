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
    /// Represents a controller for exposing endpoints relating to MessageTypes
    /// </summary>
    public class MessageTypeController : BaseController<MessageType>  
	{
		public MessageTypeController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get MessageType")]
        public override MessageType Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query MessageType")]
        public override IEnumerable<MessageType> Get() => base.Get();

        [RequiresPermission("Create MessageType")]
        public override ICollection<string> Post(ICollection<MessageType> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update MessageType")]
        public override MessageType Put(string id, MessageType entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete MessageType")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}