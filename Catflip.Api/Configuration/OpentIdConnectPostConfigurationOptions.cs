using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Catflip.Api.Configuration
{
    public class OpentIdConnectPostConfigurationOptions : IPostConfigureOptions<OpenIdConnectOptions>
    {
        IOptions<OpenIdClientOptions> ClientOptions { get; }
        ILogger<OpentIdConnectPostConfigurationOptions> Logger { get; }
        public OpentIdConnectPostConfigurationOptions(IOptions<OpenIdClientOptions> clientOptions, ILogger<OpentIdConnectPostConfigurationOptions> logger)
        {
            ClientOptions = clientOptions;
            Logger = logger;
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
            options.Events.OnRemoteFailure = context =>
            {
                Logger.LogError(context.Failure.Message);
                var errorInformation = ParseError(context.Failure.Message);
                context.HandleResponse();
                var url = $"/error?code={errorInformation.Code}&description={errorInformation.Description}&rrroruri={errorInformation.ErrorUri}";
                context.Response.Redirect(url);
                return Task.CompletedTask;
            };
        }

        static ErrorInformation ParseError(string error)
        {
            var codeMatch = Regex.Match(error, "error: '([^']+)");
            var descriptionMatch = Regex.Match(error, "error_description: '([^']+)");
            var linkMatch = Regex.Match(error, "error_uri: '([^']+)");

            var code = codeMatch.Success ? codeMatch.Groups[1].Value : string.Empty;
            var description = descriptionMatch.Success ? descriptionMatch.Groups[1].Value : string.Empty;
            description = string.IsNullOrWhiteSpace(description) ? error : description;
            var link = linkMatch.Success ? linkMatch.Groups[1].Value : string.Empty;

            return new ErrorInformation(code, description, link);
        }
    }

    record ErrorInformation(string Code, string Description, string ErrorUri);
}
