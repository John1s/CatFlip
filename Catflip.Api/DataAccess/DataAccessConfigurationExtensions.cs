using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Catflip.Api.DataAccess
{
    public static class DataAccessConfigurationExtensions
    {
        static bool UseInMemoryDb(IConfiguration configuration)
        {
            var value = configuration["UseInMemoryDb"];
            if(value != null && bool.TryParse(value, out bool useInMemory))
            {
                return useInMemory;
            }
            return false;
        }


        public static IServiceCollection AddCatflipDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddDbContext<CatflipDbContext>(options =>
            {
                if (UseInMemoryDb(configuration))
                {
                    options.UseInMemoryDatabase("userdb");
                }
                else
                {
                    var connectionString = configuration.GetConnectionString("UserDatabase");
                    options.UseSqlServer(connectionString,
                        sqlServerOptionsAction =>
                        {
                            sqlServerOptionsAction.EnableRetryOnFailure();
                        });
                }
            });
            return services;
        }

        public static void ApplyDatabaseMigrations(this IApplicationBuilder app, IConfiguration configuration)
        {
            if (UseInMemoryDb(configuration))
            {
                return;
            }
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<CatflipDbContext>>();
                using (var context = services.GetRequiredService<CatflipDbContext>())
                {
                    try
                    {
                        context.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while attempting to migrate the database.");
                        //If the database isn't available we can't do much else.
                        throw;
                    }

                }
            }
        }
    }
}
