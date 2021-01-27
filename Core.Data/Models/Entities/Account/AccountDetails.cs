using System.Collections.Generic;
using Core.Data.Models.Base;

namespace Core.Data.Models.Entities.Account
{
    public class AccountDetail : BaseEntity
    {
        public double AccountBalance { get; set; }
        public string Status { get; set; }
        public bool IsClosed { get; set; }
        public string ReasonForClosing { get; set; }

        public virtual ICollection<AccountPayment> Transactions { get; set; }
    }
}
