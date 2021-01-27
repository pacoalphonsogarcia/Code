using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Account
{
    public class AccountPayment : BaseEntity
    {
        public DateTime PaymentDateUtc { get; set; }
        public double Amount { get; set; }
        public string AccountDetailId { get; set; }
        [ForeignKey("AccountDetailId")]
        public AccountDetail AccountDetails { get; set; }
    }
}
