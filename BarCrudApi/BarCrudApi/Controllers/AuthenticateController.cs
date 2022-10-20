using BarCrudApi.Auth;
using BarCrudApi.Services;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using BarCrudMVC.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BarCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthenticateController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public AuthenticateController(IUserManagementService loginService)
        {
            _userManagementService = loginService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        // login con token, devuelve jwt y fecha de caducidad
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                ResponseLogin response=  await _userManagementService.Login(model);
                return (response.Token == null && response.Expiration == null) ? Unauthorized() : Ok(response);
            }
            return Unauthorized();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("register")]
        //se registra un usuario con rol normal de usuario
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerModel)
        {
            if (ModelState.IsValid) 
            {
                try
                {
                    //uso el servicio para registrar
                    if (await _userManagementService.Register(registerModel))
                    {
                        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                    }
                    //Si falla la creacion se debe a que el usuario ya existe o algun error interno
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User already exists!" });
                }
                catch (Exception)
                {
                    StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Status = "Error",
                            Message = "User creation failed!."
                        });
                }                
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response
                {
                    Status = "Error",
                    Message = "User data is not valid, check user details and try again."
                });
        }
        
        [HttpPost]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [Route("register/manager")]
        //Se registra un usuario con rol de manager
        public async Task<IActionResult> RegisterManager([FromBody] RegisterManagerViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uso el servicio para registrar
                    if (await _userManagementService.RegisterManager(registerModel))
                    {
                        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                    }
                    //Si falla la creacion se debe a que el usuario ya existe o algun error interno
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User already exists!" });
                }
                catch (Exception)
                {
                    StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Status = "Error",
                            Message = "User creation failed!."
                        });
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response
                {
                    Status = "Error",
                    Message = "User data is not valid, check user details and try again."
                });
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [Route("register/admin")]
        //Se registra un usuario con rol de admin
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uso el servicio para registrar
                    if (await _userManagementService.RegisterAdmin(registerModel))
                    {
                        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                    }
                    //Si falla la creacion se debe a que el usuario ya existe o algun error interno
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User already exists!" });
                }
                catch (Exception)
                {
                    StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Status = "Error",
                            Message = "User creation failed!."
                        });
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response
                {
                    Status = "Error",
                    Message = "User data is not valid, check user details and try again."
                });
        }
        [HttpPost]
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [Route("register/SuperAdmin")]
        //Se registra un usuario con rol de SuperAdmin
        public async Task<IActionResult> RegisterSuperAdmin([FromBody] RegisterViewModel registerModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //uso el servicio para registrar
                    if (await _userManagementService.RegisterSuperAdmin(registerModel))
                    {
                        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                    }
                    //Si falla la creacion se debe a que el usuario ya existe o algun error interno
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { Status = "Error", Message = "User already exists!" });
                }
                catch (Exception)
                {
                    StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Status = "Error",
                            Message = "User creation failed!."
                        });
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest,
                new Response
                {
                    Status = "Error",
                    Message = "User data is not valid, check user details and try again."
                });
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet]
        //Todas los usuarios esten o no con baja logica, solo superAdmins
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listProductos = await _userManagementService.GetAll();
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [Route("user/{id}")]
        [HttpGet]
        //Busco la persona, usuario y roles en base a su user id
        public async Task<IActionResult> GetOne(string id)
        {
            try
            {
                var user = await _userManagementService.GetOne(id);
                return Ok(user); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}")]
        //Edito un usuario, puede ser roles o datos
        public async Task<IActionResult> Edit([FromBody] UserStatusAdminViewModel user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _userManagementService.Edit(user) ? Ok(new Response
                    { Status = "Success", Message = "Se edito el producto con exito!." }) : throw new Exception();
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
            }
            return NotFound("El producto ingresado no es valido.");
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPost("softDelete/{id}")]
        //Se elimina logicamente una persona/usuario 
        public async Task<IActionResult> SoftDelete(string id)
        {
            try
            {
                return await _userManagementService.SoftDelete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino logicamente la persona con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
        }
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPost("restore/{id}")]
        //Recuperacion de la eliminacion logica,
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                return await _userManagementService.Restore(id) ? Ok(new Response
                { Status = "Success", Message = "Se recupero la persona con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
        }

        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpDelete("{id}")]
        //Borrado permanente de usuario/persona, solo superAdmins tienen permiso,
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                return await _userManagementService.Delete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino permanentemen el usuario y persona con extio!." }) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }          
        }
    }
}
