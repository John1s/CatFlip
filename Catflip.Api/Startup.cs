using Catflip.Api.Configuration;
using Catflip.Api.DataAccess;
using Catflip.Api.Services;
using Catflip.Api.Transformations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;

namespace Catflip.Api
{
    public class Startup
    {
        const string PolicyName = "MyPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var allowedOrigin = Configuration["AllowedOrigin"];
            if (allowedOrigin == null)
            {
                allowedOrigin = "http://localhost";
            }

            services.AddCors(options =>
            {
                options.AddPolicy(PolicyName,
                    builder =>
                    {
                        builder
                            .WithOrigins(allowedOrigin)
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catflip", Version = "v1" });
            });

            var options = new OpenIdClientOptions();
            services.Configure<OpenIdClientOptions>(Configuration.GetSection("openid"));
            services.AddSingleton<IPostConfigureOptions<OpenIdConnectOptions>, OpentIdConnectPostConfigurationOptions>();
             
            //Using same site = none  and secure policy same as request to allow the client to be on a different domain which can be useful for testing.
            //However, this requires us to use SSL and origin checking in production. 
            //Don't redirect on unauthenticated or unauthorized because we have UI for this.
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 403;
                    return Task.CompletedTask;
                };
            })
            .AddOpenIdConnect();

            services.AddCatflipDataAccess(Configuration);

            services.AddHttpClient<ICatService, CatService>(client => {
                client.BaseAddress = new Uri(Configuration["Services:Cats"] ?? "https://cataas.com");
            });

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddHttpContextAccessor();
            services.AddSingleton<ITransformationFactory, TransformationFactory>();
            services.AddDistributedMemoryCache();
            services.AddHttpClient<IDiscoveryDocumentService, DiscoveryDocumentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(PolicyName);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(opt => {
                    opt.RouteTemplate = "api/docs/swagger/{documentname}/swagger.json";
                });
                app.UseSwaggerUI(opts => {
                    opts.SwaggerEndpoint("/api/docs/swagger/v1/swagger.json", "Catflip v1");
                    opts.RoutePrefix = "api/docs/swagger";
                });
            }

            //Simple error handling middlerware that returns the error as JSON
            //This makes it easier for the developers of the Javascript clients to handler the error.
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.ApplyDatabaseMigrations();
        }
    }
}
