using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.Data.Models.Base
{
    public class BaseReferenceEntity : BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
