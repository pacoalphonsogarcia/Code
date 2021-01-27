using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class UserPermission : BaseEntity
    {
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string UserId { get; set; }
        [ForeignKey("PermissionId")]
        public Permission Permission { get; set; }
        public string PermissionId { get; set; }
        public string PermissionValue { get; set; }
    }

}
