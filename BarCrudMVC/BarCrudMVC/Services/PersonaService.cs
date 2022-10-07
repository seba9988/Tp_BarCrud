using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Newtonsoft.Json;

namespace BarCrudMVC.Services
{
	public class PersonaService: BaseService, IPersonaService
	{
        public PersonaService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor, httpClientFactory)
        { }
        //Busca personas con rol Manager que no esten con baja y no tengan un bar asignado
        public async Task<IList<PersonaViewModel>?> GetAllManagersSinBaja() 
        {
            try
            {
                var personas = new List<PersonaViewModel>();
                //busco personas manager en la api
                var response = await client.GetAsync("/api/Persona/getManagersSinBaja");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    personas = JsonConvert.DeserializeObject<List<PersonaViewModel>>(result);                    
                }
                return personas;
            }
            catch (Exception) { return null; }
        }
        //Busca con un dni una persona
        public async Task<PersonaViewModel?> GetOneByDni(string dni) 
        {
            try
            {
                var response = await client.GetAsync("/api/Persona/byDni/"+ dni);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var persona = JsonConvert.DeserializeObject<PersonaViewModel>(result);
                    return persona;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
    }
}
