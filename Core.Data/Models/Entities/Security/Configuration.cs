using System.ComponentModel.DataAnnotations;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class Configuration : BaseReferenceEntity
    {
        [Required]
        public string Value { get; set; }
    }
}
