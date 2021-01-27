using System;
using Core.Data.Handlers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Data.Attributes
{
    /// <summary>
    /// Used to secure an endpoint with role-based authorization and token-based authentication
    /// </summary>
    public class RequiresPermissionAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The permission that this attribute is guarding
        /// </summary>
        public string PermissionName { get; }
        /// <summary>
        /// The user's Id
        /// </summary>
        private string UserId { get; set; }
        public RequiresPermissionAttribute(string permissionName)
        {
            PermissionName = permissionName;
        }
        /// <summary>
        /// This gets called just before the actual method is called. We check for authentication and authorization here
        /// </summary>
        /// <param name="context">The executing context</param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var service = context.HttpContext.RequestServices;
            var securityHandler = service.GetService<SecurityHandler>();
            if (securityHandler == null) throw new Exception("Could not get service for SecurityHandler");

            // Check if authenticated
            if (!securityHandler.IsAuthenticated(context, out var userId)) throw new Exception($"UserId '{userId}' was not successfully authenticated.");
            
            // User is authenticated? Proceed to authorization
            UserId = userId;
            
            if (!securityHandler.IsAuthorized(UserId, PermissionName))
            {
                throw new Exception($"UserId '{userId}' was successfully authenticated but lacks permission '{PermissionName}'.");
            }
        }
        /// <summary>
        /// This gets called just after the actual method is called. We update the authorization and issue a new NOnce here
        /// </summary>
        /// <param name="context">The executed context</param>
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var service = context.HttpContext.RequestServices;
            var securityHandler = service.GetService<SecurityHandler>();
            if (securityHandler == null) throw new Exception("Could not get service for SecurityHandler");

            var token = context.HttpContext.Request.Headers["Authorization"].ToString();
            var nOnce = context.HttpContext.Request.Headers["NOnce"].ToString();

            // Invalidate the NOnce, issue a new NOnce, and extend the token's duration
            //step 1: Extend the token's duration
            securityHandler.ExtendTokenDuration(token);
            // step 2: Invalidate the NOnce
            securityHandler.InvalidateNOnce(nOnce);
            // step 3: Get a new NOnce
            var newNOnce = securityHandler.GenerateNOnce(UserId);
            context.HttpContext.Response.Headers["NOnce"] = newNOnce;
        }
    }
}
