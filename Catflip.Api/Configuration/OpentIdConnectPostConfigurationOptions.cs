using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;


namespace Catflip.Api.Configuration
{
    public class OpentIdConnectPostConfigurationOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        IOptions<OpenIdClientOptions> ClientOptions { get; }
        public OpentIdConnectPostConfigurationOptions(IOptions<OpenIdClientOptions> clientOptions)
        {
            ClientOptions = clientOptions;
        }

        public void PostConfigure(string name, OpenIdConnectOptions options)
        {
            var client = ClientOptions.Value;

            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.Authority = client.Authority;
            options.ClientId = client.ClientId;
            options.ClientSecret = client.ClientSecret;
            options.ResponseType = "code";
            options.RequireHttpsMetadata = true;
            options.SaveTokens = false;
            options.UseTokenLifetime = true;
            options.CallbackPath = new PathString("/api/auth/signin-oid");
            options.SignedOutCallbackPath = new PathString("/api/auth/signedout");
        }
    }
}
