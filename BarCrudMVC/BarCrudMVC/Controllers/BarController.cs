using BarCrudMVC.Models;
using BarCrudMVC.Services;
using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BarCrudMVC.Controllers
{
    public class BarController : Controller
    {
        private readonly IBarService _barService;
        private readonly IPersonaService _personaService;

        public BarController(IBarService barService, IPersonaService personaService)
        {
            _barService = barService;
            _personaService = personaService;
        }

        //Se busca y muestra datos del bar
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> VerBar(int id)
        {
            try
            {
                //busco datos de la categoria
                var producto = await _barService.GetOne(id);
                if (producto != null)
                {
                    return View(producto);
                }
                ViewBag.Fallo = "No se encontraron datos del bar.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }

        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        //Busco y traigo todas los bares con o sin baja, solo admins y superAdmins
        public async Task<IActionResult> BarAdmin()
        {
            try
            {
                var bares = await _barService.GetAll();
                return View(bares);
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se pasa a la view Add
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        public async Task<IActionResult> AddView()
        {
            try
            {
                //busco managers sin bares y que no tengan baja
                var managers = await _personaService.GetAllManagersSinBaja();
                ViewBag.Managers = managers;
                return View("Add");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se agrega un bar
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Add(BarAdminViewModel bar)
        {
            try
            {
                if (await _barService.Add(bar))
                {
                    ViewBag.Exito = "Se agrego con exito el bar!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la agregacion del bar.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busca los datos completos del bar a editar y muestra la vista editar
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        public async Task<IActionResult> EditView(int id)
        {
            try
            {
                //Busco el bar
                var bar = await _barService.GetOneAdmin(id);
                //Verifico que exista
                if (bar != null)
                {
                    //Busco managers sin bar y los agrego al ViewBag
                    var managers = await _personaService.GetAllManagersSinBaja();
                    //Busco manager del bar si lo tiene y no esta de baja, y lo agrego al listado
                    var managerBar = await _personaService.GetOneByDni(bar.ManagerDni);
                    //Si existe y no esta de baja
                    if(managerBar != null)
                        managers.Add(managerBar);
                    ViewBag.Managers = managers;
                    return View("Edit", bar);
                }
                   
                ViewBag.Fallo = "Fallo la busqueda del bar.";
                return View("Edit");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion del bar
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Edit(BarAdminViewModel bar)
        {
            try
            {
                if (await _barService.Edit(bar))
                {
                    ViewBag.Exito = "Se edito con exito el bar!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Edicion el bar.";
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
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja logica
                if (await _barService.SoftDelete(id))
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
        //Se efectua la baja permanente
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja permanente
                if (await _barService.Delete(id))
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
        //Se recupera un bar
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                //Compruebo si recupero con exito el bar
                if (await _barService.Restore(id))
                {
                    ViewBag.Exito = "Se recupero el bar con extio!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "No se pudo recuperar el bar.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Ver Bar para Admins y superAdmins
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpGet]
        public async Task<IActionResult> VerBarAdmin(int id)
        {
            try
            {
                //busco datos del bar
                var bar = await _barService.GetOneAdmin(id);
                if (bar != null)
                {
                    return View(bar);
                }
                ViewBag.Fallo = "No se encontraron datos del bar.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }

        [Authorize(Roles = ("Manager"))]
        //Busco y traigo todos los bares del manager sin baja
        //luego muestro vista de manager para administrar bares
        public async Task<IActionResult> BarManager()
        {
            try
            {
                var bares = await _barService.GetAllManager();
                return View(bares);
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
    }
}
