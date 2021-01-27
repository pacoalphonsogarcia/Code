using System;
using System.Collections.Generic;
using System.Linq;
using Core.Api.Controllers.Security;
using Core.Data.Contexts;
using Core.Data.Models.Entities.Security;
using Core.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests.API.Controllers.Security
{
    [TestClass]
    public class ConfigurationControllerTests
    {
        private readonly CoreContext _coreContext;
        private readonly ConfigurationController _controller;
        public ConfigurationControllerTests()
        {
            _coreContext = TestHelper.GetDatabase();
            _controller = new ConfigurationController(_coreContext);
        }

        [TestMethod]
        public void GetMultipleEntitiesTest()
        {
            // arrange
            var expectedEntities = _coreContext.Configurations.OrderBy(p => p.Id).ToList();
            // act
            var actualEntities = _controller.Get().OrderBy(p => p.Id).ToList();
            // assert

            Assert.AreEqual(expectedEntities.Count, actualEntities.Count);

            for (var i = 0; i < expectedEntities.Count; i++)
            {
                Assert.AreEqual(expectedEntities[i], actualEntities[i]);
            }

        }

        [TestMethod]
        public void GetSingleEntityTest()
        {
            // arrange
            var expectedEntity = _coreContext.Configurations.FirstOrDefault();
            // act
            if (expectedEntity == null) Assert.Fail("expectedEntity was null");
            var actualEntity = _controller.Get(expectedEntity.Id);
            // assert
            Assert.AreEqual(expectedEntity, actualEntity);
        }

        [TestMethod]
        public void PostTest()
        {
            // arrange
            var entityToAdd = CreateEntity();
            // act
            var actualIDs = _controller.Post(new List<Configuration> { entityToAdd });
            // assert
            var actualEntity = _coreContext.Configurations.FirstOrDefault(p => p.Id == actualIDs.ToList()[0]);
            if (actualEntity == null) Assert.Fail("actualEntity was null");
            Assert.AreEqual(actualIDs.ToList()[0], actualEntity.Id);
            //cleanup
            _coreContext.Configurations.Remove(actualEntity);
            _coreContext.SaveChanges();
        }

        [TestMethod]
        public void PutTest()
        {
            // arrange
            var entityToUpdate = CreateEntity();
            _coreContext.Configurations.Add(entityToUpdate);
            _coreContext.SaveChanges();
            var expectedEntity = _coreContext.Configurations.OrderByDescending(p => p.LastUpdatedUtc).FirstOrDefault();
            // act
            if (expectedEntity == null) Assert.Fail("expectedEntity was null");
            var actualEntity = _controller.Put(expectedEntity.Id, expectedEntity);
            // assert
            Assert.AreEqual(expectedEntity, actualEntity);
            //cleanup
            _coreContext.Configurations.Remove(actualEntity);
            _coreContext.SaveChanges();
        }

        [TestMethod]
        public void DeleteTest()
        {
            // arrange
            var entityToDelete = CreateEntity();
            _coreContext.Configurations.Add(entityToDelete);
            _coreContext.SaveChanges();
            var actualEntity = _coreContext.Configurations.OrderByDescending(p => p.LastUpdatedUtc).FirstOrDefault();
            if (actualEntity == null) Assert.Fail("expectedEntity was null");
            // act
            _controller.Delete(actualEntity.Id);
            // assert
            Assert.AreEqual(actualEntity.IsDeleted, true);
            //cleanup
            _coreContext.Configurations.Remove(actualEntity);
            _coreContext.SaveChanges();
        }
        private Configuration CreateEntity()
        {
            var newEntity = new Configuration
            {
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                Version = 1,
                Name = "Test", 
                Value = Guid.NewGuid().ToString("N")
            };
            return newEntity;
        }

    }
}