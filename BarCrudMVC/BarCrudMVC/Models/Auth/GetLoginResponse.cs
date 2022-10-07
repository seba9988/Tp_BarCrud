using Newtonsoft.Json;

namespace BarCrud.Models.Auth
{
    public class GetLoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("expiration")]
        public DateTime Expiration { get; set; }
    }
}
