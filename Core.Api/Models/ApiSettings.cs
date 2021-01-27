using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Api.Models
{
    public class ApiSettings
    {
        public string AllActionsPermissionId { get; set; }
        public bool IsAuditLogEnabled { get; set; }
        public string SuperUserId { get; set; }
        public int Rfc2898IterationsCount { get; set; }
        public string Version { get; set; }
        public int TokenRefreshDurationInMinutes { get; set; }
        public string SaltSize { get; set; }
        public int FailedLoginAttemptsBeforeLockout { get; set; }
        public string ErrorMessageTypeId { get; set; }
        public string InfoMessageTypeId { get; set; }
        public string WarningMessageTypeId { get; set; }
    }

}
