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
        public static IServiceCollection AddCatflipDataAccess(this IServiceCollection services, IConfiguration Configuration)
        {

            var connectionString = Configuration.GetConnectionString("UserDatabase");
            services.AddDbContext<CatflipDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    sqlServerOptionsAction =>
                    {
                        sqlServerOptionsAction.EnableRetryOnFailure();
                    });
            });
            return services;
        }

        public static void ApplyDatabaseMigrations(this IApplicationBuilder app)
        {

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
