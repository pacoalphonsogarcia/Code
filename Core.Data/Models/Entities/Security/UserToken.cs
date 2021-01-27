using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models.Base;
using Core.Data.Models.Entities.Security;

namespace Core.Data.Models.Entities.Security
{
    public class UserToken : BaseEntity
    {
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDateUtc { get; set; }
        public byte[] Value { get; set; }
    }

}
