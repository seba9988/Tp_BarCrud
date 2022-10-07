using BarCrudApi.Auth;
using BarCrudApi.Services;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BarCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.User)]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet("{userId}")]
        //Busco el pedido pendiente del usuario 
        public async Task<IActionResult> GetPedidoPendiente (string userId) 
        {
            try
            {
                var pedidoPendiente = await _pedidoService.GetPedidoPendiente(userId);

                if(pedidoPendiente != null) 
                {
                    //Verifico stock, si no hay se envia status 409 sin el pedido
                    if ( _pedidoService.VerificarStock(pedidoPendiente))
                    {
                        //Inicializo el pedidoViewModel
                        var pedido = new PedidoViewModel(pedidoPendiente);
                        //devuelvo Ok con el pedido
                        return Ok(pedido);
                    }
                    //no hay stock, status 409
                    return StatusCode(StatusCodes.Status409Conflict, new Response
                    { Status = "Error", Message = "No hay stock en alguno de los productos." });
                }
                //Si no se encontro el pedido devuelvo status Notfound
                return NotFound("El usuario no es valido o no tiene un pedido");
            } 
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }          
        }

        [Authorize(Roles = UserRoles.User)]
        [Route("historialUser/{userId}")]
        [HttpGet]
        //Todas los Productos  esten o no con baja logica, solo admins y superAdmins
        public async Task<IActionResult> GetHistorialUsuario(string userId)
        {
            try
            {
                var listProductos = await _pedidoService.GetHistorialUsuario(userId);
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }

        [Authorize(Roles = UserRoles.User)]
        [HttpGet("detalle/{pedidoId}")]
        public async Task<IActionResult> GetOnePedido(int pedidoId)
        {
            try
            {
                var categoria = await _pedidoService.GetOnePedido(pedidoId);

                return categoria != null ? Ok(categoria) : StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
        }

        [Authorize(Roles = UserRoles.Manager)]
        [Route("historialBar/{barId}")]
        [HttpGet]
        //Todas los Productos  esten o no con baja logica, solo admins y superAdmins
        public async Task<IActionResult> GetHistorialBar(int barId)
        {
            try
            {
                var listProductos = await _pedidoService.GetHistorialBar(barId);
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [HttpPost]
        //Agrego un producto al pedido del usuario
        public async Task<IActionResult> Add(CarritoAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _pedidoService.Add(model) ? Ok(new Response
                    { Status = "Success", Message = "Se agrego el producto con exito!." }) : 
                    StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
            }
            return NotFound("El producto ingresado no es valido.");
        }

        [HttpPut("{userId}")]
        //Se realiza la compra del pedido pendiente del usuario
        public async Task<IActionResult> Compra(string userId)
        {
            try
            {
                return await _pedidoService.Compra(userId) ? Ok(new Response
                { Status = "Success", Message = "Se edito el producto con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }

        [HttpPut]
        //Remuevo un producto del pedido del usuario
        public async Task<IActionResult> RemoveProducto(CarritoRemoveViewModel model)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    return await _pedidoService.RemoveProducto(model) ? Ok(new Response
                    { Status = "Success", Message = "Se removio el producto del pedido con exito!." }) : 
                    StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
                return NotFound("No se encontro el producto o el usuario");               
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }         
        }

        [HttpDelete("{userId}")]
        //Rremuevo todos los productos del pedido pendiente y lo borro
        public async Task<IActionResult> CancelarPedido(string userId)
        {
            try
            {
                return await _pedidoService.CancelarPedido(userId) ? Ok(new Response
                { Status = "Success", Message = "Se cancelo el pedido con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
    }
}
