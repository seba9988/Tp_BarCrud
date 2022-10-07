using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BarCrudMVC.Services
{
	public class CategoriaService: BaseService, ICategoriaService
	{
		public CategoriaService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor, httpClientFactory)
		{ }
        //Busco las categorias que no tengan baja
        public async Task<List<CategoriaViewModel>?> GetAllSinBaja()
		{
            try 
            {
                var categorias = new List<CategoriaViewModel>();
                //Busco categorias sin baja en la api
                var response = await client.GetAsync("/api/Categoria");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    categorias = JsonConvert.DeserializeObject<List<CategoriaViewModel>>(result);                    
                }
                return categorias;
            }
            catch (Exception) { return null; }
        }
        public async Task<List<CategoriaAdminViewModel>?> GetAll() 
        {
            try 
            {
                var categorias = new List<CategoriaAdminViewModel>();
                //Busco las categorias con y sin bajas
                var response = await client.GetAsync("/api/Categoria/allConBaja");
                response.EnsureSuccessStatusCode();
                if(response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    categorias = JsonConvert.DeserializeObject<List<CategoriaAdminViewModel>>(result);                   
                }
                return categorias;
            }
            catch(Exception) { return null; }
        }
        //busco con un id una categoria
        public async Task<CategoriaAdminViewModel?> GetOne(int id) 
        {
            try 
            {
                //Busco la categoria correspondiente al id
                var response = await client.GetAsync("/api/Categoria/" + id);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var categoria = JsonConvert.DeserializeObject<CategoriaAdminViewModel>(result);
                    return categoria;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Aplico la edicion a la categoria
        public async Task<bool> Add(CategoriaAdminViewModel categoriaVM)
        {
            try
            {
                //Hago el add en la api
                var response = await client.PostAsJsonAsync($"/api/Categoria", categoriaVM);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Aplico la edicion a la categoria
        public async Task<bool> Edit(CategoriaAdminViewModel categoriaVM) 
        {
            try
            {
                //Envio la categoria a la api para ser editada
                var response = await client.PutAsJsonAsync($"/api/Categoria/{categoriaVM.Id}", categoriaVM);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Baja logica
        public async Task<bool> SoftDelete(int id)
        {
            try
            {
                //Realizo la baja logica en la api
                var response = await client.PostAsJsonAsync($"/api/Categoria/softDelete/{id}","");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        public async Task<bool> Restore(int id)
        {
            try
            {
                //Realizo la recuperacion logica en la api
                var response = await client.PostAsJsonAsync($"/api/Categoria/restore/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        public async Task<bool> Delete(int id) 
        {
            try
            {
                //Realizo la baja logica en la api
                var response = await client.DeleteAsync($"/api/Categoria/{id}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
    }
}
