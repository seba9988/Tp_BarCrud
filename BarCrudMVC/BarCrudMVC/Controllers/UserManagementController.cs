using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using BarCrud.Models.Auth;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using BarCrudMVC.Models.Auth;
using System.Net.Http.Json;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;
using BarCrudMVC.Models;
using BarCrudMVC.Services.Interfaces;
using BarCrudMVC.Services;


namespace BarCrudMVC.Controllers
{   
    public class UserManagementController : Controller
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IBarService _barService;
        public UserManagementController(IUserManagementService userManagementService, IBarService barService )
        { 
            _userManagementService = userManagementService;
            _barService = barService;   
        }
        //lleva a la pagina de login
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    //si se loguea con exito al usuario se devuelve a la pagina home
                    if (await _userManagementService.Login(loginModel))
                        return RedirectToAction("Index", "Home");
                    //Si falla el login debido a credenciales incorrectas se devuelve a la pagina login con aviso
                    ViewBag.Message = "User name o Password incorrectos.";
                    return View("Index");
                }
                ViewBag.Message = "Los datos ingresados no corresponden a credenciales correctas.";
                return View();
            }
            catch (Exception) 
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }         
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            try
            {
                if (ModelState.IsValid == true)
                {
                    if (await _userManagementService.Register(registerModel))
                    {
                        ViewBag.Exito = "Se registro con Exito!";
                        return View();
                    }
                }
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }
        }
        
        [HttpGet]
        public  IActionResult Register()
        {
            return View();
        }

        //Desloguea al usuario
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _userManagementService.LogOut();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }
        }
        //Busca datos del usuario en el token, y trae de la tabla personas su dni
        [Authorize]
        public async Task<IActionResult> UserStatus()
        {
            try
            {
                var userData = await _userManagementService.UserStatus();
                //Si se trae con exito los datos del usuario logueado y los muestro
                if (userData != null)
                {
                    return View(userData);
                }
                //si no se encuentran los datos del usuario quiere decir que tiene algun error
                //se desloguea el usuario
                return RedirectToAction("LogOut", "Authenticate");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }
        }

        //Se busca bares sin manager y sin baja si los hay , luego se redirecciona a la pagina de registrar manager
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpGet]
        public async Task<IActionResult> RegisterManager()
        {
            try 
            {
                var bares = await _barService.GetAllSinManager();
                ViewBag.Bares = bares;
                return View();
            }
            catch (Exception) 
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }         
        }
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            try 
            {
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }
        }
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpGet]
        public IActionResult RegisterSuperAdmin()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error, intentelo mas tarde.";
                return View("AccionResult");
            }
        }
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> RegisterManager(RegisterManagerViewModel registerModel)
        {
            try
            {
                if (ModelState.IsValid == true)
                {
                    if (await _userManagementService.RegisterManager(registerModel))
                    {
                        ViewBag.Exito = "Se registro con Exito!";
                        return View();
                    }
                }
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View("AccionResult");
            }
        }
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel registerModel)
        {
            try
            {
                if (ModelState.IsValid == true)
                {
                    if (await _userManagementService.RegisterAdmin(registerModel))
                    {
                        ViewBag.Exito = "Se registro con Exito!";
                        return View();
                    }
                }
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View("AccionResult");
            }
        }
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpPost]
        public async Task<IActionResult> RegisterSuperAdmin(RegisterViewModel registerModel)
        {
            try
            {
                if (ModelState.IsValid == true)
                {
                    if (await _userManagementService.RegisterSuperAdmin(registerModel))
                    {
                        ViewBag.Exito = "Se registro con Exito!";
                        return View();
                    }
                }
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Fallo el registro, vuelva a Intentarlo.";
                return View("AccionResult");
            }
        }
        [Authorize(Roles = ("SuperAdmin"))]
        //Busco y traigo todos los usuarios con o sin baja, solo admins y superAdmins
        public async Task<IActionResult> UserAdmin()
        {
            try
            {
                var users = await _userManagementService.GetAll();
                return View(users);
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se busca datos del usuario seleccionado
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpGet]
        public async Task<IActionResult> VerUserAdmin(string id)
        {
            try
            {
                //busco datos del usuario , persona y sus roles
                var user = await _userManagementService.GetOne(id);
                if (user != null)
                {
                    return View(user);
                }
                ViewBag.Fallo = "No se encontraron datos del usuario.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busca los datos completos usuario a editar
        [Authorize(Roles = ("SuperAdmin"))]
        public async Task<IActionResult> EditView(string id)
        {
            try
            {
                var user = await _userManagementService.GetOne(id);
                if (user != null)
                {
                    return View(user);
                }
                ViewBag.Fallo = "Fallo la busqueda del usuario.";
                return View("Edit");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion del usuario
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpPost]
        public async Task<IActionResult> Edit(UserStatusAdminViewModel user)
        {
            try
            {
                if (await _userManagementService.Edit(user))
                {
                    ViewBag.Exito = "Se edito con exito el usuario!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Edicion del usuario.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la baja logica
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpPost]
        public async Task<IActionResult> SoftDelete(string id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja logica
                if (await _userManagementService.SoftDelete(id))
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
        //Lo que implica que tambien se va a borrar pedidos y detalle pedido de este usuario
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja permanente
                if (await _userManagementService.Delete(id))
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
        //Se recupera un usuario de la baja logica
        [Authorize(Roles = ("SuperAdmin"))]
        [HttpPost]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                //Compruebo si recupero con exito el usuario
                if (await _userManagementService.Restore(id))
                {
                    ViewBag.Exito = "Se recupero el usuario con extio!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "No se pudo recuperar el usuario.";
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

