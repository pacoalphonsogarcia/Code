using System;
using System.Linq;
using System.Security.Cryptography;
using Core.Data.Contexts;
using Core.Data.Handlers;
using Core.Data.Models.Entities.Security;
using Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.Data.Handlers
{
    [TestClass]
    public class SecurityHandlerTests
    {
        private readonly CoreContext _coreContext;
        private readonly SecurityHandler _securityHandler;
        private const string SuperUserId = "SuperUser";
        private const string Credentials = "YXBwaWQ6ZGVmYXVsdGFwcAp1c2VybmFtZTpzdXBlcnVzZXIKcGFzc3dvcmQ6cmVhbGx5QmFkSGFyZGNvZGVkUGFzc3dvcmQ=";

        public SecurityHandlerTests()
        {
            _coreContext = TestHelper.GetDatabase();
            _securityHandler = new SecurityHandler(_coreContext);

        }
        [TestMethod]
        public void AuthenticateTest()
        {
            // arrange
            // credentials comes from base64(appid:defaultapp\nusername:SuperUser\npassword:reallyBadHardcodedPassword), the default values when the database is seeded
            var newCryptographicRandomBytes = GenerateCryptographicRandomBytes();

            // create and save a token
            var newToken = new UserToken
            {
                ExpiryDateUtc = DateTime.UtcNow.AddMinutes(10), IsDeleted = false, LastUpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"), Version = 1,
                UserId = SuperUserId, Value = newCryptographicRandomBytes
            };
            _coreContext.UserTokens.Add(newToken);
            _coreContext.SaveChanges();
            // act
            var tokenNOnceToken = _securityHandler.Authenticate(Credentials, Convert.ToBase64String(newCryptographicRandomBytes), out _);
            // assert
            Assert.AreNotEqual(tokenNOnceToken, null);
            // clean
            _coreContext.UserTokens.Remove(newToken);
            _coreContext.SaveChanges();
        }

        [TestMethod]
        public void ExtendTokenDurationTest()
        {
            // arrange
            // create a token
            var newCryptographicRandomBytes = GenerateCryptographicRandomBytes();
            var utcNow = DateTime.UtcNow;
            var newToken = new UserToken
            {
                ExpiryDateUtc = utcNow,
                IsDeleted = false,
                LastUpdatedUtc = utcNow,
                Id = Guid.NewGuid().ToString("N"),
                Version = 1,
                UserId = SuperUserId,
                Value = newCryptographicRandomBytes
            };
            _coreContext.UserTokens.Add(newToken);
            _coreContext.SaveChanges();
            // act
            _securityHandler.ExtendTokenDuration(Convert.ToBase64String(newCryptographicRandomBytes));
            // get back the token
            var actualToken = _coreContext.UserTokens.FirstOrDefault(p => p.Value == newCryptographicRandomBytes);
            if (actualToken == null) Assert.Fail("actualToken was null");
            // assert
            Assert.AreEqual(actualToken.ExpiryDateUtc.Minute, utcNow.AddMinutes(10).Minute);
            //clean
            _coreContext.UserTokens.Remove(newToken);
            _coreContext.SaveChanges();
        }

        [TestMethod]
        public void GenerateNOnceTest()
        {
            // arrange
            var nOnce = _securityHandler.GenerateNOnce(SuperUserId);
            // act
            var actualnOnce = _coreContext.NOnces.OrderByDescending(p => p.LastUpdatedUtc)
                .FirstOrDefault(p => p.UserId == SuperUserId);
            if (actualnOnce == null) Assert.Fail("actualnOnce was null");
            // assert
            Assert.AreEqual(nOnce, Convert.ToBase64String(actualnOnce.Value));
            //clean
            _coreContext.NOnces.Remove(actualnOnce);
            _coreContext.SaveChanges();
        }
        private static byte[] GenerateCryptographicRandomBytes(int size = 64)
        {
            var cryptographicBytes = new byte[size];
            var csp = new RNGCryptoServiceProvider();
            csp.GetBytes(cryptographicBytes);
            return cryptographicBytes;
        }
    }
}
