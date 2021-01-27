using System;
using System.Collections.Generic;
using System.Linq;
using Core.Api.Controllers.Account;
using Core.Data.Contexts;
using Core.Data.Models.Entities.Account;
using Core.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.API.Controllers.Account
{
    [TestClass]
    public class AccountDetailsControllerTests
    {
        private readonly CoreContext _coreContext;
        private readonly AccountDetailsController _accountDetailsController;
        public AccountDetailsControllerTests()
        {
            _coreContext = TestHelper.GetDatabase();
            _accountDetailsController = new AccountDetailsController(_coreContext);
        }
        [TestMethod]
        public void GetMultipleAccountDetailsTest()
        {
            // arrange
            var expectedAccountDetails = _coreContext.AccountDetails.Include(p => p.Transactions).ToList();
            expectedAccountDetails = expectedAccountDetails.OrderByDescending(p => p.Transactions.Select(q => q.PaymentDateUtc)).ToList();
            // act
            var actualAccountDetails = _accountDetailsController.Get().ToList();
            // assert
            Assert.AreEqual(expectedAccountDetails.Count, actualAccountDetails.Count);
            var expectedAccountTransactions = expectedAccountDetails.Select(p => p.Transactions).ToList();
            var actualAccountTransactions = actualAccountDetails.Select(p => p.Transactions).ToList();
            // assert each AccountDetail are equal and in the same order
            for (var i = 0; i < expectedAccountDetails.Count; i++)
            {
                Assert.AreEqual(expectedAccountDetails[i], actualAccountDetails[i]);
            }
            // assert each AccountDetail's transactions are equal and in the same order
            for (var i = 0; i < expectedAccountTransactions.ToList().Count; i++)
            {
                Assert.AreEqual(expectedAccountTransactions[i], actualAccountTransactions[i]);
            }
        }

        [TestMethod]
        public void GetSingleAccountDetailTest()
        {
            // arrange
            // get any one account detail
            var expectedAccountDetail = _coreContext.AccountDetails.Include(p => p.Transactions).FirstOrDefault();
            if (expectedAccountDetail == null) Assert.Fail("No test data available for AccountDetail");
            // act
            var actualAccountDetail = _accountDetailsController.Get(expectedAccountDetail.Id);
            // assert
            Assert.AreEqual(expectedAccountDetail, actualAccountDetail);
            var expectedAccountTransactions = expectedAccountDetail.Transactions.ToList();
            var actualAccountTransactions = actualAccountDetail.Transactions.ToList();
            for (var i = 0; i < expectedAccountDetail.Transactions.ToList().Count; i++)
            {
                Assert.AreEqual(expectedAccountTransactions[i], actualAccountTransactions[i]);
            }
        }
            
        [TestMethod]
        public void PostTest()
        {
            var sentinelFilter = Guid.NewGuid().ToString("N");
            // arrange
            var newAccountDetail = CreateAccountDetail();
            newAccountDetail.ReasonForClosing = sentinelFilter;
            // act
            var expectedAccountDetail = _accountDetailsController.Post(new List<AccountDetail> {newAccountDetail}).ToList();
            var actualAccountDetail = _coreContext.AccountDetails.FirstOrDefault(p =>
                p.AccountBalance == 12.34 &&
                p.IsClosed == false &&
                p.IsDeleted == false &&
                p.ReasonForClosing == sentinelFilter &&
                p.Status == "New Account" &&
                p.Version == 1
            );
            // assert
            if (actualAccountDetail == null) Assert.Fail("actualAccountDetail was null");
            Assert.AreEqual(expectedAccountDetail[0], actualAccountDetail.Id);
            // cleanup
            _coreContext.AccountDetails.Remove(actualAccountDetail);
            _coreContext.SaveChanges();
        }

        [TestMethod]
        public void PutTest()
        {
            var putTest = "Put test"; 
            var putAssertionTest = "Put assertion test";
            // arrange
            var newAccountDetail = CreateAccountDetail();
            newAccountDetail.Status = putTest;
            newAccountDetail.Id = Guid.NewGuid().ToString("N");
            _coreContext.AccountDetails.Add(newAccountDetail);
            _coreContext.SaveChanges();
            var expectedAccountDetail = _coreContext.AccountDetails.FirstOrDefault(
                p => p.Status == putTest);
            if (expectedAccountDetail == null) Assert.Fail("ExpectedAccountDetail was null");
            expectedAccountDetail.Status = putAssertionTest;
            // act
            var actualAccountDetail =
                _accountDetailsController.Put(expectedAccountDetail.Id, expectedAccountDetail);
            // assert
            Assert.AreEqual(actualAccountDetail.Status, putAssertionTest);
            // cleanup
            _coreContext.AccountDetails.Remove(actualAccountDetail);
            _coreContext.SaveChanges();
        }

        [TestMethod]
        public void DeleteTest()
        {
            // arrange
            var idToDelete = Guid.NewGuid().ToString("N");
            var newAccountDetail = CreateAccountDetail();
            newAccountDetail.Id = idToDelete;
            _coreContext.AccountDetails.Add(newAccountDetail);
            _coreContext.SaveChanges();
            // act
            _accountDetailsController.Delete(idToDelete);
            // assert
            var deletedAccountDetail = _accountDetailsController.Get(idToDelete);
            Assert.AreEqual(deletedAccountDetail, null);
            // cleanup
            _coreContext.AccountDetails.Remove(newAccountDetail);
            _coreContext.SaveChanges();
        }
        private static AccountDetail CreateAccountDetail()
        {
            var newAccountDetail = new AccountDetail
            {
                AccountBalance = 12.34,
                IsClosed = false,
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                Status = "New Account",
                Version = 1
            };
            return newAccountDetail;
        }
    }
}