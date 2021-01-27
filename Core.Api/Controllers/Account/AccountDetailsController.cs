using System.Collections.Generic;
using System.Linq;
using Core.Api.Controllers.Base;
using Core.Data.Attributes;
using Microsoft.AspNetCore.Mvc;
using Core.Data.Contexts;
using Core.Data.Models.Entities.Account;
using Microsoft.AspNet.OData;
using Microsoft.EntityFrameworkCore;

namespace Core.Api.Controllers.Account
{
    /// <summary>
    /// Represents a controller for exposing endpoints relating to AccountDetails
    /// </summary>
	public class AccountDetailsController : BaseController<AccountDetail>  
	{   
        private CoreContext Context { get; }

        public AccountDetailsController(CoreContext context) : base(context)
        {
            Context = context;
        }

        [HttpGet("{id}")]
        [EnableQuery]
        [RequiresPermission("Get Account")]
        public override AccountDetail Get(string id)
        {
            var accountDetail = Context.AccountDetails.Include(p => p.Transactions)
                .FirstOrDefault(p => p.Id.ToLower() == id.ToLower() && p.IsDeleted == false);

            if (accountDetail == null) return null;

            accountDetail.Transactions =
            accountDetail.Transactions.OrderByDescending(p => p.PaymentDateUtc).ToList();
            return accountDetail;

        }

        [HttpGet]
        [EnableQuery(PageSize = 10)]
        [RequiresPermission("Query Account")]
        public override IEnumerable<AccountDetail> Get() => base.Get();

        [RequiresPermission("Create Account")]
        public override ICollection<string> Post(ICollection<AccountDetail> values)
        {
            return base.Post(values);
        }

        [HttpPut("{id}")]
        [RequiresPermission("Update Account")]
        public override AccountDetail Put(string id, AccountDetail entity)
        {
            return base.Put(id, entity);
        }

        [HttpDelete("{id}")]
        [RequiresPermission("Delete Account")]
        public override void Delete(string id)
        {
            base.Delete(id);
        }
	}
}



