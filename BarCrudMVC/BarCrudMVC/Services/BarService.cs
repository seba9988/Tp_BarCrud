using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Newtonsoft.Json;

namespace BarCrudMVC.Services
{
    public class BarService : BaseService, IBarService
    {
        public BarService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor, httpClientFactory)
        { }
        //Busca todos los bares, esten con baja logica o no
        public async Task<IList<BarAdminViewModel>?> GetAll() 
        {
            try
            {
                var bares = new List<BarAdminViewModel>();
                //Busco los bares con y sin bajas
                var response = await client.GetAsync("/api/Bar/allConBaja");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    bares = JsonConvert.DeserializeObject<List<BarAdminViewModel>>(result);                   
                }
                return bares;
            }
            catch (Exception) { return null; }
        }
        //Busca todos los bares que no esten de baja, fechaBaja=null
        public async Task<IList<BarViewModel>> GetAllSinBaja() 
        {
            try 
            {
                var bares = new List<BarViewModel>();
                //busco bares sin baja en la api
                var response = await client.GetAsync("/api/Bar");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    bares = JsonConvert.DeserializeObject<List<BarViewModel>>(result);
                }
                return bares;
            }
            catch (Exception) 
            {
                return null;
            }           
        }
        //Busca todos los bares que no esten de baja, fechaBaja=null
        public async Task<IList<BarViewModel>> GetAllSinManager()
        {
            try 
            {
                var bares = new List<BarViewModel>();
                var response = await client.GetAsync("/api/Bar/sinManager");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    bares = JsonConvert.DeserializeObject<List<BarViewModel>>(result);
                }
                return bares;
            }
            catch (Exception) { return null; }           
        }
        
        //Trae todos los datos basicos de bar
        public async Task<BarViewModel?> GetOne(int id) 
        {
            try
            {
                //Busco el bar correspondiente al id
                var response = await client.GetAsync("/api/Bar/" + id);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var bar = JsonConvert.DeserializeObject<BarViewModel>(result);
                    return bar;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Trae todos los datos de un bar, solo para admins
        public async Task<BarAdminViewModel?> GetOneAdmin(int id) 
        {
            try
            {
                //Busco el bar correspondiente al id
                var response = await client.GetAsync("/api/Bar/admin/" + id);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var bar = JsonConvert.DeserializeObject<BarAdminViewModel>(result);
                    return bar;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Agrega un bar nueva si este es valido para guardar,false si no se agrego, true si se agrega
        public async Task<bool> Add(BarAdminViewModel bar) 
        {
            try
            {
                //Hago el add en la api
                var response = await client.PostAsJsonAsync($"/api/Bar", bar);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
    
        //Edita un bar si esta es valido para editar,false si no se edito, true si se edita
        public async Task<bool> Edit(BarAdminViewModel barEditado) 
        {
            try
            {
                //Envio el bar a la api para ser editado
                var response = await client.PutAsJsonAsync($"/api/Bar/{barEditado.Id}", barEditado);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Elimina un bar logicamente si este es valido para eliminar,false si no se elimino, true si se elimina
        public async Task<bool> SoftDelete(int id)
        {
            try
            {
                //Realizo la baja logica en la api
                var response = await client.PostAsJsonAsync($"/api/Bar/softDelete/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Recupera un bar eliminado logicamente si estr es valido para recuperar,false si no se recupera, true si se recupera
        public async Task<bool> Restore(int id)
        {
            try
            {
                //Realizo la recuperacion logica en la api
                var response = await client.PostAsJsonAsync($"/api/Bar/restore/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }

        //Elimina un bar permanentemente si este es valido para eliminar,false si no se elimino, true si se elimina
        public async Task<bool> Delete(int id) 
        {
            try
            {
                //Realizo la baja logica en la api
                var response = await client.DeleteAsync($"/api/Bar/{id}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Busca todos los bares sin baja del manager logueado actualmente
        public async Task<BarViewModel?> GetAllManager() 
        {
            try 
            {
                //Busco id del manager logueado actualmente
                var userId = _contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                if (userId != null)
                {
                    //Busco los bares administrados por el manager de id userId
                    var response = await client.GetAsync("/api/Bar/allManager/" + userId);
                    response.EnsureSuccessStatusCode();
                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        var bares = JsonConvert.DeserializeObject<BarViewModel>(result);
                        return bares;
                    }                 
                }
                return null;
            }
            catch (Exception) { return null; }
        }
    }
}
