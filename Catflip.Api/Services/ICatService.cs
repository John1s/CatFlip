using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Catflip.Api.Services
{
    public interface ICatService
    {
        Task<byte[]> GetCat(string tag);
    }

    class CatService: ICatService
    {
        HttpClient HttpClient { get; }
        public CatService(HttpClient httpClient)
        {
            HttpClient = httpClient;

        }

        public async Task<byte[]> GetCat(string tag)
        {
            var url = string.IsNullOrWhiteSpace(tag) ? "/cat" : $"/cat/{tag}";
            using (var message = new HttpRequestMessage(HttpMethod.Get, url))
            {
                using (var response = await HttpClient.SendAsync(message))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsByteArrayAsync();
                }
            }
        }
    }
}
