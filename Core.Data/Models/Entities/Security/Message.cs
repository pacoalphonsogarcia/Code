using System.ComponentModel.DataAnnotations.Schema;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Security
{
    public class Message : BaseReferenceEntity
    {
        public string Description { get; set; }

        [ForeignKey("MessageTypeId")]
        public MessageType MessageType { get; set; }

        public string MessageTypeId { get; set; }
    }
}
