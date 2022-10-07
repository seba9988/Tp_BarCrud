using BarCrudMVC.CustomException;
using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BarCrudMVC.Controllers
{
    [Authorize(Roles = ("User"))]
    public class PedidoController : Controller
	{
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        //busco el pedido del usuario,
        //si tiene uno lo envio a la vista carrito con  su lista productos
        [HttpGet]
        public async Task <IActionResult> CarritoView()
        {
            try
            {
                var carrito = await _pedidoService.GetPedidoPendiente();

                if (carrito != null)
                {
                    return View(carrito);
                }
                ViewBag.Mensaje = "Este usuario no tiene un pedido armado.";
                return View("CarritoView", carrito);
            }
            catch (SinStockException ex) 
            {
                ViewBag.Error = ex.Message;
                return View("CarritoView");
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busco el historial de pedidos de un usuario
        [HttpGet]
        public async Task<IActionResult> VerHistorialUsuario()
        {
            try
            {
                //busco historial pedidos del usuario
                var pedidos = await _pedidoService.GetHistorialUsuario();

                if (pedidos != null)
                {
                    return View("VerHistorial",pedidos);
                }
                ViewBag.Fallo = "No se encontro el pedido del usuario.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busco el pedido de un usuario
        [HttpGet]
        public async Task<IActionResult> VerDetalle(int Id)
        {
            try
            {
                //busco historial pedidos del usuario
                var pedido = await _pedidoService.GetOnePedido(Id);

                if (pedido != null)
                {
                    return View("VerDetalle", pedido);
                }
                ViewBag.Fallo = "No se encontro el pedido del usuario.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busco el historial de pedidos de un bar, solo manager del bar puede verlo
        [Authorize(Roles = ("Manager"))]
        [HttpGet]
        public async Task<IActionResult> VerHistorialBar(string nombre, int barId)
        {
            try
            {
                //busco historial pedidos del usuario
                var pedidos = await _pedidoService.GetHistorialBar(barId);

                if (pedidos != null )
                {
                    ViewBag.Nombre = nombre;
                    return View("VerHistorialBar", pedidos);
                }
                ViewBag.Mensaje = "No se encontro el pedido del bar.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Error = "Ocurrio un error.";
                return View("AccionResult");
            }
        }

        //Se agrega un producto al carrito/pedido,
        //si el usuario tiene un pedido pendiente sin comprar se agrega el producto a este,
        //de lo contrario se crea un pedido nuevo y se agrega,
        //si el pedido ya tiene el producto se le suma cantidad si esta no supera el maximo de stock del producto
        [Authorize(Roles = ("User"))]
        [HttpPost]
        public async Task<IActionResult> Add(CarritoAddViewModel productoAgregar)
        {
            try
            {
                if (await _pedidoService.Add(productoAgregar))
                {
                    ViewBag.Exito = "Se Agrego el producto al carrito!.";
                    return RedirectToAction("CarritoView","Pedido");
                }
                ViewBag.Fallo = "Fallo la agragacion del producto, vuelva a intentarlo.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se remueve un producto del pedido/carrito del usuario actual si este se encuentra en su lista
        [Authorize(Roles = ("User"))]
        [HttpPost]
        public async Task<IActionResult> RemoveProducto(int Id)
        {
            try
            {
                var productoRemover = new CarritoRemoveViewModel { ProductoId = Id };
                if (await _pedidoService.RemoveProducto(productoRemover))
                {
                    ViewBag.Exito = "Se removio el producto del carrito con exito!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la eliminacion de producto del carrito.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la compra del pedido con la lista de productos en el carrito
        [Authorize(Roles = ("User"))]
        [HttpPost]
        public async Task<IActionResult> Compra()
        {
            try
            {
                if (await _pedidoService.Compra())
                {
                    ViewBag.Exito = "Se compro realizo la compra con exito!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la comrpa, vuelva a intentarlo.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }      
        //Se cancela el pedido, para ello se borra pedido con sus detalles
        [Authorize(Roles = ("User"))]
        [HttpPost]
        public async Task<IActionResult> CancelarPedido()
        {
            try
            {
                if (await _pedidoService.CancelarPedido())
                {
                    return RedirectToAction("CarritoView", "Pedido");
                }
                ViewBag.Fallo = "No se pudo cancelar el pedido, vuelva a intentarlo.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
    }
}
