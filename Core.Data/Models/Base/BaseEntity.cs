using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Data.Models.Base
{
    public class BaseEntity
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        [Required]
        public DateTime LastUpdatedUtc { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Version { get; set; }
    }
}
