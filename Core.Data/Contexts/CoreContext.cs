using System;
using System.Collections.Generic;
using System.Linq;
using Core.Data.Contracts;
using Core.Data.Handlers;
using Core.Data.Models.Entities.Account;
using Core.Data.Models.Entities.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Data.Contexts
{
    /// <summary>
    /// Represents the model of the database
    /// </summary>
    public class CoreContext : DbContext, IContext
    {
        public CoreContext(DbContextOptions<CoreContext> contextOptions) : base(contextOptions) { }
        /// <summary>
        /// Populates the data store with seed values
        /// </summary>
        public void Seed()
        {
            if (DatabaseExists()) return;
            // we make sure that the database is created; no use running the web API without a backing data store
            Database.EnsureCreated();
            SeedConfigurations();
            SeedMessageTypes();
            SaveChanges();
            SeedMessages();
            SeedClients();
            SeedRoles();
            SaveChanges();
            SeedPermissions();
            SaveChanges();
            SeedUsers();
            SaveChanges();
            SeedUserPermissions();
            SaveChanges();
            SeedUserTokens();
            SaveChanges();
            SeedApps();
            SaveChanges();
            SeedNOnces();
            SaveChanges();
            SeedAccountDetails();
            SaveChanges();
        }

        private void SeedUserTokens()
        {
            UserTokens.Add(new UserToken
            {
                ExpiryDateUtc = DateTime.UtcNow.AddMinutes(10),
                Id = Guid.NewGuid().ToString(),
                IsDeleted = false, 
                LastUpdatedUtc = DateTime.UtcNow,
                UserId = "SuperUser",
                Version = 1,
                Value = SecurityHandler.GenerateCryptographicRandomBytes()
            });
        }
        private void SeedMessages()
        {
            Messages.Add(new Message
            {
                Description = $"Web app deployed on {DateTime.UtcNow} UTC time",
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                MessageTypeId = "InfoMessageType",
                Name = "First deployment",
                Version = 1
            });
            
        }

        private void SeedNOnces()
        {
            NOnces.AddRange(new NOnce
            {
                ExpiryDateUtc = DateTime.UtcNow.AddYears(1),
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                User = Users.First(p => p.Id == "SuperUser"),
                Version = 1,
                UserId = Users.First(p => p.Id == "SuperUser").Id
            });
        }

        private void SeedClients()
        {
            Clients.Add(new Client
            {
                Id = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                Version = 1
            });
        }
        private void SeedApps()
        {
            var app = new App
            {
                Id = "defaultapp",
                Description = "Default App",
                IsDeleted = false,
                LastUpdatedUtc = DateTime.UtcNow,
                Secret = Guid.NewGuid().ToString("N"),
                Version = 1,
                Users = new List<User>()
            };
            app.Users.Add(Users.Single(p => p.Id == "SuperUser"));
            Apps.Add(app);

        }

        private void SeedMessageTypes()
        {
            MessageTypes.AddRange(
                new MessageType
                {
                    Id = "ErrorMessageType",
                    Name = "ErrorMessageType",
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new MessageType
                {
                    Id = "InfoMessageType",
                    Name = "InfoMessageType",
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow
                },
                new MessageType
                {
                    Id = "WarningMessageType",
                    Name = "WarningMessageType",
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow
                });

        }

        private void SeedConfigurations()
        {
            Configurations.AddRange(
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "IsAuditLogEnabled",
                    Value = "1",
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "Version",
                    Value = "0.0.0.1"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "TokenRefreshDurationInMinutes",
                    Value = "10"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "FailedLoginAttemptsBeforeLockout",
                    Value = "5"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "SaltSize",
                    Value = "512"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "Rfc2898IterationCount",
                    Value = "10000"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "SuperUserId",
                    Value = "SuperUser"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "AllActionsPermissionId",
                    Value = "AllActions"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "ErrorMessageTypeId",
                    Value = "ErrorMessageType"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "InfoMessageTypeId",
                    Value = "InfoMessageType"
                },
                new Configuration
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1,
                    Name = "WarningMessageTypeId",
                    Value = "WarningMessageType"
                }
            );
        }

        private void SeedRoles()
        {
            Roles.AddRange(
                new Role
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Name = "Administrator",
                    Version = 1
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Name = "SuperUser",
                    Version = 1
                },
                new Role
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Name = "Default",
                    Version = 1
                }
            );
        }

        private void SeedPermissions()
        {
            Permissions.AddRange(
                new Permission()
                {
                    Name = "Administrator.AllActions",
                    Id = "AllActions",
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Version = 1
                });
        }

        private void SeedUsers()
        {
            // TODO: Do not use really bad hard coded passwords
            var reallyBadHardcodedPassword = @"reallyBadHardcodedPassword";
            var passwordHashAndSalt = SecurityHandler.CreatePasswordHashAndSalt(reallyBadHardcodedPassword);

            Users.Add(new User()
            {
                EmailAddress = "superuser@superuser.com",
                UserKey = Guid.NewGuid().ToString("N"),
                IsDeleted = false,
                Id = "SuperUser",
                LastUpdatedUtc = DateTime.UtcNow,
                FailedLoginAttempts = 0,
                IsEmailConfirmed = true,
                IsLockedOut = false,
                IsLockoutEnabled = false,
                Username = "superuser",
                Version = 1,
                Salt = passwordHashAndSalt.Item2,
                PasswordHash = passwordHashAndSalt.Item1,
                Role = Roles.Single(p => p.Name == "Administrator"),
            }
            );
            passwordHashAndSalt = SecurityHandler.CreatePasswordHashAndSalt(reallyBadHardcodedPassword);

            Users.Add(
                new User()
                {
                    EmailAddress = "defaultuser@defaultuser.com",
                    UserKey = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    Id = "Default",
                    LastUpdatedUtc = DateTime.UtcNow,
                    FailedLoginAttempts = 0,
                    IsEmailConfirmed = true,
                    IsLockedOut = false,
                    IsLockoutEnabled = false,
                    Username = "defaultuser",
                    Version = 1,
                    Salt = passwordHashAndSalt.Item2,
                    PasswordHash = passwordHashAndSalt.Item1,
                    Role = Roles.Single(p => p.Name == "Default")
                }
            );
        }

        private void SeedUserPermissions()
        {
            UserPermissions.AddRange(
                new UserPermission
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    UserId = Users.FirstOrDefault(p => p.Id == "SuperUser")?.Id,
                    PermissionId = Permissions.FirstOrDefault(p => p.Name == "Administrator.AllActions")?.Id,
                    PermissionValue = "1",
                    Version = 1
                }
                //,
                //new UserPermission
                //{
                //    Id = Guid.NewGuid().ToString("N"),
                //    IsDeleted = false,
                //    LastUpdatedUtc = DateTime.UtcNow,
                //    UserId = Users.FirstOrDefault(p => p.Id == "Default")?.Id,
                //    PermissionId = Permissions.FirstOrDefault(p => p.Name == "AccountPayments.")?.Id,
                //    PermissionValue = "1",
                //    Version = 1
                //}
            );
        }

        private void SeedAccountDetails()
        {
            var accountDetailId = Guid.NewGuid().ToString("N");
            AccountDetails.Add(new AccountDetail
            {
                AccountBalance = 1837.24, Id = accountDetailId, IsDeleted = false, IsClosed = false, ReasonForClosing = "", Status = "Active",
                LastUpdatedUtc = DateTime.UtcNow, Version = 1, Transactions = new List<AccountPayment>()
                {
                    new()
                    {
                        Amount = 12.34, Id = Guid.NewGuid().ToString("N"), IsDeleted = false,
                        LastUpdatedUtc = DateTime.UtcNow, PaymentDateUtc = DateTime.UtcNow, Version = 1,
                        AccountDetailId = accountDetailId
                    },
                    new ()
                    {
                        Amount = 5.12, Id = Guid.NewGuid().ToString("N"), IsDeleted = false,
                        LastUpdatedUtc = DateTime.UtcNow, PaymentDateUtc = DateTime.UtcNow.AddDays(-1), Version = 1,
                        AccountDetailId = accountDetailId
                    },
                    new ()
                    {
                        Amount = 12.34, Id = Guid.NewGuid().ToString("N"), IsDeleted = false,
                        LastUpdatedUtc = DateTime.UtcNow, PaymentDateUtc = DateTime.UtcNow.AddDays(-2), Version = 1,
                        AccountDetailId = accountDetailId
                    }
                }
            });
        }

        public DbSet<App> Apps { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<Configuration> Configurations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        public DbSet<NOnce> NOnces { get; set; }
        public DbSet<AccountDetail> AccountDetails { get; set; }
        public DbSet<AccountPayment> AccountPayments { get; set; }

        public bool DatabaseExists()
        {
            return this.GetService<IDatabaseCreator>() is RelationalDatabaseCreator relationalDatabaseCreator &&
                   relationalDatabaseCreator.Exists();
        }
    }
}
