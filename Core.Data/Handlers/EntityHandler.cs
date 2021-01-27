using System;
using Core.Data.Models.Base;
using Core.Data.Models.Entities.Security;

namespace Core.Data.Handlers
{
    /// <summary>
    /// Helper class for providing useful methods to handle entities
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public static class DataHelper<T> where T : BaseEntity
    {

        public static void AddDefaultUserValues(ref User userEntity)
        {
            userEntity.Id = Guid.NewGuid().ToString("N");
            userEntity.IsDeleted = false;
            userEntity.LastUpdatedUtc = DateTime.UtcNow;
            userEntity.Version = 1;
            userEntity.FailedLoginAttempts = 0;
            userEntity.IsEmailConfirmed = false;
            userEntity.IsLockedOut = false;
            userEntity.IsLockoutEnabled = true;
        }

    }
}
