using System.Collections.Generic;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class Client : BaseEntity
    {
        public string ClientKey { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
