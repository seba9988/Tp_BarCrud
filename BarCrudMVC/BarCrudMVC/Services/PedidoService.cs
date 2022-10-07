using BarCrudMVC.CustomException;
using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Newtonsoft.Json;
using System.Net;

namespace BarCrudMVC.Services
{
    public class PedidoService : BaseService, IPedidoService
    {
        public PedidoService(IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor) :
            base(httpContextAccessor, httpClientFactory)
        { }
        //Busca pedido pendiente del usuario
        public async Task<PedidoViewModel?> GetPedidoPendiente()
        {
            try
            {              
                //busco id del usuario logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                //Busco los bares con y sin bajas
                var response = await client.GetAsync($"/api/Pedido/{userId}");
                var statusCodes = response.StatusCode;
                //response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var pedido = JsonConvert.DeserializeObject<PedidoViewModel>(result);
                    return pedido;
                }   
                if((int)response.StatusCode == 409) 
                {
                    await CancelarPedido();
                    throw new SinStockException("Se cancelo el pedido debido a falta de stock.");
                }
                //si fallo y no es statusCode 409 devuelvo null
                return null;
            }
            catch (SinStockException) 
            {
                //Si tengo esta excepcion la devuelvo al controlador para que cancele pedido                
                throw;
            }
            catch (Exception) { return null; }
        }
        //Busca historial de pedidos de un usuario
        public async Task<IList<PedidoHistorialViewModel>?> GetHistorialUsuario() 
        {
            try
            {
                var pedidos = new List<PedidoHistorialViewModel>();
                //busco id del usuario logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                //Hago el add en la api
                var response = await client.GetAsync($"/api/Pedido/historialUser/{userId}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    pedidos = JsonConvert.DeserializeObject<List<PedidoHistorialViewModel>>(result);
                }
                return pedidos;
            }
            catch (Exception) { return null; }
        }
        //Busca todos pedidoDetalle de pedidos finalizados de un bar, con sus productos y bar
        public async Task<IList<PedidoDetalleBarViewModel>?> GetHistorialBar(int barId) 
        {
            try
            {
                var pedidos = new List<PedidoDetalleBarViewModel>();
                //Hago el add en la api
                var response = await client.GetAsync($"/api/Pedido/historialBar/{barId}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    pedidos = JsonConvert.DeserializeObject<List<PedidoDetalleBarViewModel>>(result);                  
                }
                return pedidos;
            }
            catch (Exception) { return null; }
        }

        //Busca todos los datos de un pedido
        public async Task<PedidoViewModel?> GetOnePedido(int pedidoId) 
        {
            try
            {
                //Hago el add en la api
                var response = await client.GetAsync($"/api/Pedido/detalle/{pedidoId}");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var pedido = JsonConvert.DeserializeObject<PedidoViewModel>(result);
                    return pedido;

                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Agrega un bar nueva si este es valido para guardar,false si no se agrego, true si se agrega
        public async Task<bool> Add(CarritoAddViewModel model)
        {
            try
            {
                //busco id del usuario logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                model.UserId = userId;
                //Hago el add en la api
                var response = await client.PostAsJsonAsync($"/api/Pedido", model);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }

        //Edita un bar si esta es valido para editar,false si no se edito, true si se edita
        public async Task<bool> Compra()
        {
            try
            {
                //busco id del usuario logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                //Envio el bar a la api para ser editado
                var response = await client.PutAsJsonAsync($"/api/Pedido/{userId}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Remueve un producto del pedido
        public async Task<bool> RemoveProducto(CarritoRemoveViewModel model)
        {
            try
            {
                //busco id del usuario logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;
                model.UserId = userId;
                //Realizo la baja en la api
                var response = await client.PutAsJsonAsync($"/api/Pedido", model);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Elimina un bar permanentemente si este es valido para eliminar,false si no se elimino, true si se elimina
        public async Task<bool> CancelarPedido()
        {
            try
            {
                //busco id del usuario logueado
                var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;

                //Realizo la baja en la api
                var response = await client.DeleteAsync($"/api/Pedido/{userId}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
    }
}
