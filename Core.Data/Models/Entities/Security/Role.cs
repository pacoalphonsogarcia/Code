using System.Collections.Generic;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class Role : BaseReferenceEntity
    {
        public virtual ICollection<UserPermission> RolePermissions { get; set; }
    }

}
