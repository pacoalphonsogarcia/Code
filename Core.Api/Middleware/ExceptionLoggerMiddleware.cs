using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Core.Api.Models;
using Core.Data.Contexts;
using Core.Data.Models.Entities.Security;
using Microsoft.AspNetCore.Http;

namespace Core.Api.Middleware
{
    /// <summary>
    /// Custom middleware used for logging exceptions to the data store
    /// </summary>
    public class ExceptionLoggerMiddleware
    {
        private ExceptionDispatchInfo _exceptionInfo;
        private readonly RequestDelegate _next;
        private CoreContext _coreContext;
        private readonly ApiSettings _apiSettings;
        public ExceptionLoggerMiddleware(RequestDelegate next, ApiSettings apiSettings)
        {
            _next = next;
            _apiSettings = apiSettings;
        }

        public async Task InvokeAsync(HttpContext context, CoreContext coreContext)
        {
            _coreContext = coreContext;
            try
            {
                 await _next.Invoke(context);
            }
            // anytime an exception occurs anywhere in the Web API, this gets called
            catch (Exception ex)
            {
                // we use ExceptionDispatchInfo so we can save the original Stack trace of the error in the data store
                _exceptionInfo = ExceptionDispatchInfo.Capture(ex);
                HandleExceptionAsync(context);
            }
        }

        private void HandleExceptionAsync(HttpContext context)
        {
            var message = new Message
            {
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                Version = 1,
                LastUpdatedUtc = DateTime.UtcNow,
                MessageTypeId = _apiSettings.ErrorMessageTypeId,
                Name = $"'{_exceptionInfo.SourceException.Message}' occurred while trying to access '{context.Request.Host}', action: '{context.Request.Method}', querystring: {context.Request.QueryString.Value}",
                Description = $"Inner exception: {_exceptionInfo.SourceException.InnerException}, Stack Trace: {_exceptionInfo.SourceException.StackTrace}"
            };
            _coreContext.Messages.Add(message);
            _coreContext.SaveChanges();
        }
    }
}
