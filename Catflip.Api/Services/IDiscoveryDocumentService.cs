
using Catflip.Api.Configuration;
using IdentityModel.Client;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Catflip.Api.Services
{
    public interface IDiscoveryDocumentService
    {
        Task<bool> SupportsFederatedLogout();
    }

    class DiscoveryDocumentService : IDiscoveryDocumentService
    {
        HttpClient Client { get; }
        IOptions<OpenIdClientOptions> Options { get; }
        ILogger<DiscoveryDocumentService> Logger { get; }
        IDistributedCache DocumentCache { get; }
        public DiscoveryDocumentService(HttpClient client, IDistributedCache documentCache, IOptions<OpenIdClientOptions> options)
        {
            Client = client;
            DocumentCache = documentCache;
            Options = options;
        }


        public async Task<DiscoveryDocumentResponse> DiscoveryDocument(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                throw new ArgumentNullException("discoveryDocument");
            }
            try
            {
                var discoveryRequest = new DiscoveryDocumentRequest()
                {
                    Address = url,
                    Policy = new DiscoveryPolicy
                    {
                        ValidateEndpoints = false
                    }
                };

                var disco = await Client.GetDiscoveryDocumentAsync(discoveryRequest);
                if (disco.IsError)
                {
                    throw new Exception(disco.Error);
                }
                return disco;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> SupportsFederatedLogout()
        {
            var authority = Options.Value.Authority;
            if(string.IsNullOrWhiteSpace(authority))
            {
                return false;
            }
            var cached = await DocumentCache.GetStringAsync(authority);

            if(cached == null)
            {
                var docuemnt = await DiscoveryDocument(authority); 
                var supportsSignout =  docuemnt != null && !string.IsNullOrWhiteSpace(docuemnt.EndSessionEndpoint);
                await DocumentCache.SetStringAsync(authority, JsonConvert.SerializeObject(supportsSignout));
                return supportsSignout;
            }else
            {
                return JsonConvert.DeserializeObject<bool>(cached);
            }
        }
    }
}
