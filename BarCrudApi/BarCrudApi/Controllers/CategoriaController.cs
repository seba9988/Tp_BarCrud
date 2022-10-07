using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace BarCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;
        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        //Todas las Categorias que no esten con baja
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSinBaja()
        {
            try
            {
                var listCategorias = await _categoriaService.GetAllSinBaja();
                return Ok(listCategorias);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }

        #region Admins y SuperAdmins

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [Route("allCategoriasProductos")]
        [HttpGet]
        //Todas las Categorias con sus productos esten o no con baja logica, solo admins y superAdmins
        public async Task<IActionResult> GetAllConBajaYProd()
        {
            try
            {
                var listCategorias = await _categoriaService.GetAllCategoriasProductos();
                return Ok(listCategorias); ;
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
        //todas las categorias sin productos esten o no con baja logica, solo admins y superAdmins
        public async Task<IActionResult> GetConFechaBaja()
        {
            try
            {
                var listCategorias = await _categoriaService.GetAll();

                return Ok(listCategorias);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id) 
        {
            
            try
            {
                CategoriaAdminViewModel categoria = new CategoriaAdminViewModel { Id = id };

                categoria = await _categoriaService.GetOne(categoria);

                return categoria != null ? Ok(categoria): StatusCode(StatusCodes.Status500InternalServerError, 
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
        //Agrego una categoria nueva, solo admins y superAdmin tienen permiso
        public async Task<IActionResult> Add(CategoriaViewModel categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _categoriaService.Add(categoria) ? Ok(new Response
                    { Status = "Success", Message = "Se agrego la nueva Categoria con exito!." }) :
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
        //Edito una categoria, solo admins y superAdmin tienen permiso
        public async Task<IActionResult> Edit([FromBody] CategoriaViewModel categoriaEditada)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _categoriaService.Edit(categoriaEditada) ? Ok(new Response
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
        //Recuperacion de la eliminacion logica, tambien se recuperan todos los productos de la categoria
        //pensar como mantener el estado de los productos antes de la eliminacion logica para mantener todo igual
        public async Task<IActionResult> Restore(int id)
        {      
            try
            {
                return await _categoriaService.Restore(id) ? Ok(new Response
                { Status = "Success", Message = "Se recupero la Categoria con exito!." }) :
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
        //Se elimina logicamente una categoria y sus productos solo admins y superAdmin tienen permiso
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                return await _categoriaService.SoftDelete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino logicamente la Categoria con exito!." }) : 
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
        //Borrado permanente de categorias, solo admins y superAdmins tienen permiso,
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await _categoriaService.Delete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino permanentemen la Categoria y sus productos con exito!." }) : 
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
        #endregion
    }
}
