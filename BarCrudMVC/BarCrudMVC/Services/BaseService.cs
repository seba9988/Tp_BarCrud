using System.Net.Http.Headers;

namespace BarCrudMVC.Services
{
	//Clase base para todos los servicios, instancia el clienteFActory con su base uri
	//Tambien se agrega jwt al header si este esta en las coockies
	public class BaseService
	{
		protected readonly IHttpContextAccessor _contextAccessor;
		protected readonly HttpClient client;
        //El token se va a estar usando en la capa de servicio,
        //por lo que creo su nombre en una constante por si luego lo quiero cambiar
        public const string XAccessToken = "X-Access-Token";
        public BaseService(IHttpContextAccessor contextAccessor,
			IHttpClientFactory httpClientFactory)
		{
            _contextAccessor = contextAccessor;
            //creo instancia de client
            client = httpClientFactory.CreateClient("BarApi");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //busco token
            var token = _contextAccessor.HttpContext.Request
				.Cookies[XAccessToken];
			//si el token no es null lo agrego al header
			if(!string.IsNullOrWhiteSpace(token))
			{
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("bearer", token);
            }
        }
	}
}
