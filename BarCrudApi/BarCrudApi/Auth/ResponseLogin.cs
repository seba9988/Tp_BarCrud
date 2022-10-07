using NuGet.Common;

namespace BarCrudApi.Auth
{
    public class ResponseLogin
    {
        public String? Token { get; set; }
        public DateTime? Expiration { get; set; }
    }
}
