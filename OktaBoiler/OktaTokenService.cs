using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using OktaBoiler.Infrastructure;
using System.Text;
using System.Text.Json;

namespace OktaBoiler
{
    public class OktaTokenService : IOktaTokenService
    {
        private const string OKTA_TOKEN_NAME = "OktaTokenService.TokenCacheKey";
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;

        public OktaTokenService(ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _logger = loggerFactory.CreateLogger<OktaTokenService>();
            _httpClient = httpClientFactory.CreateClient("OktaTokenService");
            _memoryCache = memoryCache;
        }
        public async Task<string> FetchTokenAsync()
        {
            return (await GetWithCache(OKTA_TOKEN_NAME)).AccessToken;
        }
        private async Task<OktaToken> GetWithCache(string cacheKey)
        {
            if (_memoryCache.TryGetValue<CacheItem<OktaToken?>>(cacheKey, out var cacheItem))
            {
                _logger.LogInformation("Token retrieved from Cache.");
                if (cacheItem.Created.Add(TimeSpan.FromSeconds(cacheItem!.Value!.ExpiresIn)) > DateTime.Now)
                {
                    //Token is valid
                    _logger.LogInformation("Token is NOT expired.");
                    return cacheItem.Value;
                }

                _logger.LogInformation("Token is EXPIRED.");
            }

            return await MintNewToken();
        }
        private async Task<OktaToken> MintNewToken()
        {
            _logger.LogInformation("Attempting to mint new Token.");

            var response = await _httpClient.PostAsync("", new StringContent("grant_type=client_credentials&scope=machine", Encoding.UTF8, "application/x-www-form-urlencoded"));
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            var newToken = JsonSerializer.Deserialize<OktaToken>(result);
            if (newToken != null)
            {
                var newCacheItem = new CacheItem<OktaToken>(newToken);
                _memoryCache.Set(OKTA_TOKEN_NAME, newCacheItem);
                _logger.LogInformation("Minted new Token. Placed in cache.");

                return newToken;
            }
            else
            {
                throw new Exception($"Failed to retrieve token from {_httpClient.BaseAddress}");
            }
        }
    }
}
