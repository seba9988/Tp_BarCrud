using BarCrud.Models.Auth;
using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;

namespace BarCrudMVC.Controllers
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        //Busco y traigo todas las categorias con o sin baja, solo admins y superAdmins
        public async Task<IActionResult> CategoriaAdmin()
        {
            try
            {
                var categorias = await _categoriaService.GetAll();
                return View(categorias);
            }
            catch (Exception) 
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se pasa a la view Add
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        public  IActionResult AddView()
        {
            try
            {
                return View("Add");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion de la categoria
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Add(CategoriaAdminViewModel categoria)
        {
            try
            {
                if (await _categoriaService.Add(categoria))
                {
                    ViewBag.Exito = "Se edito con exito la categoria!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la agragacion de la categoria.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busca los datos completos de la categoria a editar y muestra la vista editar
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        public async Task<IActionResult> EditView(int id)
        {
            try
            {
                var categoria = await _categoriaService.GetOne(id);
                if (categoria != null)
                    return View("Edit",categoria);
                ViewBag.Fallo = "Fallo la busqueda de la categoria.";
                return View("Edit");
            }
            catch (Exception) 
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion de la categoria
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Edit(CategoriaAdminViewModel categoria)
        {
            try
            {
                if(await _categoriaService.Edit(categoria))
                {
                    ViewBag.Exito = "Se edito con exito la categoria!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Edicion de la categoria.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion de la categoria
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpGet]
        public async Task<IActionResult> VerCategoria(int id)
        {
            try
            {
                //busco datos de la categoria
                var categoria = await _categoriaService.GetOne(id);
                if (categoria != null)
                {
                    return View(categoria);
                }
                ViewBag.Fallo = "No se encontraron datos de la categoria.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la baja logica
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja logica
                if (await _categoriaService.SoftDelete(id))
                {
                    ViewBag.Exito = "Se realizo la baja logica con extio!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "No se pudo realizar la baja logica.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la baja permanente, esta baja es en cascada
        //Lo que significa que tambien se va a borrar productos, pedidos de ese producto
        //Y tambien pedido detalle
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult>Delete(int id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja permanente
                if (await _categoriaService.Delete(id))
                {
                    ViewBag.Exito = "Se realizo la baja permanente con extio!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "No se pudo realizar la baja permanente.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la baja logica
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Retrieve(int id)
        {
            try
            {
                //Compruebo si recupero con exito la categoria
                if (await _categoriaService.Restore(id))
                {
                    ViewBag.Exito = "Se recupero la categoria con extio!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "No se pudo recuperar la categoria.";
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
