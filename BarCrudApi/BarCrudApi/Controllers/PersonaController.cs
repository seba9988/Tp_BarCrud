using BarCrudApi.Auth;
using BarCrudApi.Models;
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
    [Authorize]
    public class PersonaController : ControllerBase
    {
        private readonly IPersonaService _personaService;

        public PersonaController(IPersonaService personaService)
        {
            _personaService = personaService;
        }

        [HttpGet("{id}")]
        //Trae datos de la persona con un IdUsuario
        public async Task<IActionResult> PersonaUser(string id)
        {
            try
            {
                var datosPersona = await _personaService.GetOneByIdUser(id);

                return ModelState.IsValid ? Ok(datosPersona):
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [HttpGet("byDni/{dni}")]
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        //Trae datos de la persona con un dni
        public async Task<IActionResult> GetOneByDni(string dni)
        {
            try
            {
                var datosPersona = await _personaService.GetOneByDni(dni);

                return ModelState.IsValid ? Ok(datosPersona) :
                StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [Route("getManagersSinBaja")]
        [HttpGet]
        //Todas las personas con rol manager sin baja y que no tengan asignado un bar
        public async Task<IActionResult> GetAllManagersSinBaja()
        {
            try
            {
                var listProductos = await _personaService.GetAllManagersSinBaja();
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
    }
}
