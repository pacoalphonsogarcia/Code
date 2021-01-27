using System.Collections.Generic;
using Core.Api.Controllers.Base;
using Core.Data.Attributes;
using Microsoft.AspNetCore.Mvc;
using Core.Data.Contexts;
using Core.Data.Handlers;
using Core.Data.Models.Entities.Security;
using Microsoft.AspNet.OData;

namespace Core.Api.Controllers.Security
{
    /// <summary>
    /// Represents a controller for exposing endpoints relating to Users
    /// </summary>
    public class UserController : BaseController<User>  
	{
        private SecurityHandler SecurityHandler { get; set; }

        public UserController(CoreContext context) : base(context)
        {
            SecurityHandler = new SecurityHandler(context);
        }
		
        [HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get User")]
        public override User Get(string id)
        {
            return base.Get(id);
        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query User")]
        public override IEnumerable<User> Get() => base.Get();

        [RequiresPermission("Create User")]
        public override ICollection<string> Post(ICollection<User> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update User")]
        public override User Put(string id, User entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete User")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }
        /// <summary>
        /// Logs in the user. If successful, the authorization and NOnce tokens are sent back in the Authorization and Credentials headers.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Authorize")]
        public IActionResult Authorize()
        {
            // credentials should have the format: <base64(appid:<appid>\nusername:<username>\npassword:<password>)>
            var token = Request.Headers["Authorization"];
            var credentials = Request.Headers["Credentials"];
            var authenticationResult = SecurityHandler.Authenticate(credentials, token, out var errors);
            // With token? Extend duration
            if (authenticationResult != null)
            {
                // Authenticated? return back the token key to be used for the subsequent request
                Response.Headers["Authorization"] = authenticationResult.Item1;
                Response.Headers["NOnce"] = authenticationResult.Item2;
                return Ok();
            }

            return Unauthorized(string.Join(',', errors));
        }

    }
}



