using System.Collections.Generic;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class App : BaseEntity
    {
        public string Description { get; set; }
        public string Secret { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }

}
