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
    /// Represents a controller for exposing endpoints relating to UserTokens
    /// </summary>
    public class UserTokenController : BaseController<UserToken>  
	{
        public UserTokenController(CoreContext context) : base(context) { }

		[HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get UserToken")]
        public override UserToken Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query UserToken")]
        public override IEnumerable<UserToken> Get() => base.Get();

        [RequiresPermission("Create UserToken")]
        public override ICollection<string> Post(ICollection<UserToken> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update UserToken")]
        public override UserToken Put(string id, UserToken entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete UserToken")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }

	}
}



