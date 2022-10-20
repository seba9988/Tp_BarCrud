using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.Services;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BarCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarController : ControllerBase
    {
        private readonly IBarService _barService;
        public BarController(IBarService barService)
        {
            _barService = barService;
        }

        //Todos los bares que no esten con baja
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSinBaja()
        {
            try
            {
                var listBares = await _barService.GetAllSinBaja();
                return Ok(listBares);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
     
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [Route("allConBaja")]
        [HttpGet]
        //Todos los bares con sus productos esten o no con baja logica, solo admins y superAdmins
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listBares = await _barService.GetAll();
                return Ok(listBares); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [Route("sinManager")]
        [HttpGet]
        //Tods los bares que no tengan baja y manager asignado, solo admins y superAdmins
        public async Task<IActionResult> GetAllSinManager()
        {
            try
            {
                var listBares = await _barService.GetAllSinManager();
                return Ok(listBares); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        //busco los datos basicos de un bar
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {

            try
            {
                var bar = await _barService.GetOne(id);

                return bar != null ? Ok(bar) : StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
        }
        //busco todos los datos de un bar
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpGet("admin/{id}")]
        public async Task<IActionResult> GetOneAdmin(int id)
        {

            try
            {
                var bar = await _barService.GetOneAdmin(id);

                return bar != null ? Ok(bar) : StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPost]
        //Agrego un bar nuevo, solo admins y superAdmin tienen permiso
        public async Task<IActionResult> Add(BarAdminViewModel bar)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _barService.Add(bar) ? Ok(new Response
                    { Status = "Success", Message = "Se agrego el nuevo bar con exito!." }) :
                    StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
            }
            return NotFound("Los datos no son validos.");
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPut("{id}")]
        //Edito un bar, solo admins y superAdmin tienen permiso
        public async Task<IActionResult> Edit([FromBody] BarAdminViewModel barEditado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _barService.Edit(barEditado) ? Ok(new Response
                    { Status = "Success", Message = "Se edito la Categoria con exito!." }) :
                    StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
            }
            return NotFound("El id ingresado no exite!");
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPost("restore/{id}")]
        //Recuperacion de la eliminacion logica
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                return await _barService.Restore(id) ? Ok(new Response
                { Status = "Success", Message = "Se recupero el bar con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            //aca podria hacer un catch de una custom excepcion para detectar que el id no exite
            //return NotFound("El id ingresado no exite!");
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPost("softDelete/{id}")]
        //Se elimina logicamente un bar y sus productos solo admins y superAdmin tienen permiso
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                return await _barService.SoftDelete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino logicamente el bar con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            //aca podria hacer un catch de una custom excepcion para detectar que el id no exite
            //return NotFound("El id ingresado no exite!");       
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpDelete("{id}")]
        //Borrado permanente de bar, solo admins y superAdmins tienen permiso,
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await _barService.Delete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino permanentemen el bar con exito!." }) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            //aca podria hacer un catch de una custom excepcion para detectar que el id no exite
            //return NotFound("El id ingresado no exite!");           
        }

        [Authorize(Roles = UserRoles.Manager)]
        [HttpGet("allManager/{id}")]
        //Busco bares sin baja del id correspondiente a un manager
        public async Task<IActionResult> GetAllManager(string id) 
        {
            try
            {
                var listBares = await _barService.GetAllManager(id);
                return listBares != null ? Ok(listBares):
                    StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." }); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
    }
}
