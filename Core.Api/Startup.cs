using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Api.Middleware;
using Core.Api.Models;
using Core.Data.Attributes;
using Core.Data.Contexts;
using Core.Data.Handlers;
using Core.Data.Models.Entities.Security;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace Core.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();
            //Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var initializationSettings = new InitializationSettings();
            var apiSettings = new ApiSettings();

            var defaultConfigSection = Configuration.GetSection("Initialization");
            var apiSettingsConfigSection = Configuration.GetSection("ApiSettings");
            InitializeInitializationSettings(ref initializationSettings, defaultConfigSection);
            InitializeApiSettings(ref apiSettings, apiSettingsConfigSection);
            services
                .AddDbContext<CoreContext>(options => options.UseSqlServer(defaultConfigSection["CoreConnectionString"]))
                // to use Sqlite as the backing database instead of SqlServer, uncomment the below line of code and comment out the above
                //.AddDbContext<CoreContext>(options => options.UseSqlite(defaultConfigSection["SqliteCoreConnectionString"]))

                .AddSingleton(initializationSettings)
                .AddSingleton(apiSettings)
                .AddScoped<SecurityHandler>()

                .Configure<InitializationSettings>(Configuration.GetSection("Initialization"));

            // If using Kestrel:
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // If using IIS:
            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            // allow OData querying
            services.AddOData();
            services
                .AddMvc(options =>
                {
                    options.EnableEndpointRouting = false;
                    //options.Filters.Add<PermissionFilter>();

                }).AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                    options.JsonSerializerOptions.DictionaryKeyPolicy = null;
                })

                .AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CoreContext coreContext)
        {
            app
                .UseCors(builder => builder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin());

            SeedDatabases(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ExceptionLoggerMiddleware>();
            app.UseMvc(
                routeBuilder =>
                {
                    // for OData
                    routeBuilder.EnableDependencyInjection();
                    routeBuilder
                        .Expand() // allow deep level data retrieval
                        .Select() // allow selecting of specific data
                        .OrderBy() // allow the ordering of data
                        .Filter(); // allow filtering of data
                }
                );
            InitializeApiSettings(coreContext);
        }

        private static void SeedDatabases(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var coreContent = serviceScope.ServiceProvider.GetService<CoreContext>();
            coreContent?.Seed();
        }

        private void InitializeInitializationSettings(ref InitializationSettings settings, IConfigurationSection config)
        {
            settings.CoreConnectionString = config["CoreConnectionString"];
            settings.DefaultAuthorizationScheme = config["DefaultAuthorizationScheme"];
            settings.ShouldInitializeDatabase = bool.Parse(config["ShouldInitializeDatabase"]);
        }
        private void InitializeApiSettings(CoreContext coreContext)
        {
            var currentAssembly = Assembly.GetExecutingAssembly();

            // automatically detect permissions that have been added to controllers
            // and then save those permissions in the database if they do not yet exist

            var discoveredActionsWhichRequirePermissions = currentAssembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(RequiresPermissionAttribute), true).Length > 0)
                .Select(p => p.GetCustomAttribute(typeof(RequiresPermissionAttribute)))
                .ToList();

            var discoveredPermissions = discoveredActionsWhichRequirePermissions
                .Select(p => ((RequiresPermissionAttribute)p).PermissionName).ToList();

            var permissionsFromDatabase = coreContext.Permissions.Select(p => p.Name).ToList();
            foreach (var discoveredPermission in discoveredPermissions.Where(discoveredPermission =>
                !permissionsFromDatabase.Contains(discoveredPermission)))
            {
                coreContext.Permissions.Add(new Permission
                {
                    Id = Guid.NewGuid().ToString("N"),
                    IsDeleted = false,
                    LastUpdatedUtc = DateTime.UtcNow,
                    Name = discoveredPermission,
                    Version = 1,
                    Description = $"Auto discovered permission: {discoveredPermission}"
                });
            }

            coreContext.SaveChanges();
        }

        private void InitializeApiSettings(ref ApiSettings apiSettings, IConfigurationSection config)
        {
            // general configurations
            apiSettings.AllActionsPermissionId = config["AllActionsPermissionId"];
            apiSettings.IsAuditLogEnabled = bool.Parse(config["IsAuditLogEnabled"]);
            apiSettings.SuperUserId = config["SuperUserId"];
            apiSettings.Rfc2898IterationsCount = int.Parse(config["Rfc2898IterationCount"]);
            apiSettings.Version = config["Version"];
            apiSettings.TokenRefreshDurationInMinutes = int.Parse(config["TokenRefreshDurationInMinutes"]);
            apiSettings.SaltSize = config["SaltSize"];
            apiSettings.FailedLoginAttemptsBeforeLockout = int.Parse(config["FailedLoginAttemptsBeforeLockout"]);
            apiSettings.ErrorMessageTypeId = config["ErrorMessageTypeId"];
            apiSettings.InfoMessageTypeId = config["InfoMessageTypeId"];
            apiSettings.WarningMessageTypeId = config["WarningMessageTypeId"];
        }
    }
}
