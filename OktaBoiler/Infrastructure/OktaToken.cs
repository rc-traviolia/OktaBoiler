using System.Text.Json.Serialization;

namespace OktaBoiler.Infrastructure
{
    public class OktaToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
