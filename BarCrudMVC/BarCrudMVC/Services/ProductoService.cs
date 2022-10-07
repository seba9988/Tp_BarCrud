using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Newtonsoft.Json;

namespace BarCrudMVC.Services
{
    public class ProductoService : BaseService, IProductoService
    {

        public ProductoService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor, httpClientFactory)
        { }

        //Traigo todos los productos sin baja, para usuario sin loguear o con rol usuario
        public async Task<List<ProductoViewModel>?> GetAllSinBaja() 
        {
            try
            {
                var productos = new List<ProductoViewModel>();
                //busco productos en la api
                var response = await client.GetAsync("/api/Producto");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(result);                   
                }
                return productos;
            }
            catch (Exception) { return null; }
        }
        //Busco todos los productos sin baja de una categoria
        public async Task<List<ProductoViewModel>?> GetAllByCategoria(int id) 
        {
            try
            {
                var productos = new List<ProductoViewModel>();
                //busco productos por categoria en la api
                var response = await client.GetAsync($"/api/Producto/byCategoria/{id}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(result);                    
                }
                return productos;
            }
            catch (Exception) { return null; }
        }
        //Con un id bar busco sus productos sin baja
        public async Task<IList<ProductoViewModel>?> GetAllByBar(int id)
        {
            try
            {
                var productos = new List<ProductoViewModel>();
                //busco productos de un bar sin baja
                var response = await client.GetAsync($"/api/Producto/byBar/{id}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    productos = JsonConvert.DeserializeObject<List<ProductoViewModel>>(result);                   
                }
                return productos;
            }
            catch (Exception) { return null; }
        }
        //busca productos con o sin baja, solo admins y superAdmins
        public async Task<List<ProductoAdminViewModel>?> GetAll() 
        {
            try
            {
                var productos = new List<ProductoAdminViewModel>();
                //Busco los productos con y sin bajas
                var response = await client.GetAsync("/api/Producto/allConBaja");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    productos = JsonConvert.DeserializeObject<List<ProductoAdminViewModel>>(result);                   
                }
                return productos;
            }
            catch (Exception) { return null; }
        }
        //busca un prodcuto con un id, se trae datos del producto sin fecha baja y bar
        //Si el producto esta con baja logica no se lo trae
        public async Task<ProductoViewModel?> GetOne(int id) 
        {
            try
            {
                //Busco el producto correspondiente al id
                var response = await client.GetAsync("/api/Producto/" + id);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var producto = JsonConvert.DeserializeObject<ProductoViewModel>(result);
                    return producto;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Se trae todos los datos de producto,su bar, categoria y fecha baja
        //se trae el producto este o no con baja logica
        public async Task<ProductoAdminViewModel?> GetOneAdmin(int id)
        {
            try
            {
                //Busco el producto correspondiente al id
                var response = await client.GetAsync("/api/Producto/admin/" + id);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var producto = JsonConvert.DeserializeObject<ProductoAdminViewModel>(result);
                    return producto;
                }
                return null;
            }
            catch (Exception) { return null; }
        }

        //Agrega un producto 
        public async Task<bool> Add(ProductoAdminViewModel productoVM) 
        {
            try
            {
                //Hago el add en la api
                var response = await client.PostAsJsonAsync($"/api/Producto", productoVM);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Edita un producto 
        public async Task<bool> Edit(ProductoAdminViewModel productoVM) 
        {
            try
            {          
                //envio el producto a la api para ser editado
                var response = await client.PutAsJsonAsync($"/api/Producto/{productoVM.Id}", productoVM);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Baja logica de prodcuto
        public async Task<bool> SoftDelete(int id) 
        {
            try
            {
                //Realizo la baja logica en la api
                var response = await client.PostAsJsonAsync($"/api/Producto/softDelete/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Recuperacion de la baja logica de un prodcuto
        public async Task<bool> Restore(int id) 
        {
            try
            {
                //Realizo recuperacion logica
                var response = await client.PostAsJsonAsync($"/api/Producto/restore/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Se efectua la baja permanente, en la api
        //Lo que implica que tambien se va a borrar stocks, pedidos y detalle pedido de este producto
        public async Task<bool> Delete(int id) 
        {
            try
            {
                //Realizo la baja permanente en la api
                var response = await client.DeleteAsync($"/api/Producto/{id}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }

        //Agrega un producto al bar de un manager
        public async Task<bool> Add(ProductoManagerViewModel productoVM)
        {
            try
            {
                //Busco userId del manager logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;

                productoVM.ManagerId = userId;
                //Hago el add en la api
                var response = await client.PostAsJsonAsync($"/api/Producto/manager", productoVM);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Edita un producto
        public async Task<bool> Edit(ProductoManagerViewModel productoVM)
        {
            try
            {
                //Busco userId del manager logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;

                productoVM.ManagerId = userId;
                //envio el producto a la api para ser editado
                var response = await client.PutAsJsonAsync($"/api/Producto/manager/{productoVM.Id}", productoVM);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Baja logica de prodcuto
        public async Task<bool> SoftDeleteManager(int id)
        {
            try
            {
                //Busco userId del manager logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                var productoManager = new SoftDeleteManagerViewModel
                {
                    ManagerId = userId,
                    ProductoId = id
                };
                //Realizo la baja logica en la api
                var response = await client.PostAsJsonAsync($"/api/Producto/softDelete/Manager", productoManager);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
    }
}
