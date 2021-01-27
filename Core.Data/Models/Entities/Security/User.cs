using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class User : BaseEntity
    {
        public string UserKey { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public int FailedLoginAttempts { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsLockoutEnabled { get; set; }
        public string Salt { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public string ClientId { get; set; }

        public virtual ICollection<UserPermission> UserPermissions { get; set; }

        public virtual ICollection<UserToken> UserTokens { get; set; }

        public virtual ICollection<NOnce> NOnces { get; set; }

        public virtual ICollection<App> Apps { get; set; }
    }

}
