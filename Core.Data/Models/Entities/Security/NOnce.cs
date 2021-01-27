using System;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    /// <summary>
    /// Represents an NOnce (Number used Once) token; that is, a token that can only be used once and then is thrown away.
    /// </summary>
    public class NOnce : BaseEntity
    {
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime ExpiryDateUtc { get; set; }
        public byte[] Value { get; set; }
    }
}
