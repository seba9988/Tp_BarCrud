using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.Services;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BarCrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }
           
        //Todas los productos que no esten con baja
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllSinBaja()
        {
            try
            {
                var listProductos = await _productoService.GetAllSinBaja();
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        //Busca un producto con un id, se lo trae solamente si no tiene baja
        //el producto viene con sus datos menos fecha baja y su bar
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {

            try
            {
                var categoria = await _productoService.GetOne(id);

                return categoria != null ? Ok(categoria) : StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
        }

        //Todas los productos sin baja de una categoria 
        [AllowAnonymous]
        [HttpGet("byCategoria/{id}")]
        public async Task<IActionResult> GetByCategoria(int id)
        {
            try
            {
                var listProductos = await _productoService.GetAllByCategoria(id);
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
        [AllowAnonymous]
        [HttpGet("byBar/{id}")]
        //buusco todos los productos sin baja de un bar 
        public async Task<IActionResult> GetbyBar(int id)
        {
            try
            {
                var listProductos = await _productoService.GetAllByBar(id);
                return Ok(listProductos); ;
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
        //Todas los Productos esten o no con baja logica, solo admins y superAdmins
        public async Task<IActionResult> GetAllConFechaBaja()
        {
            try
            {
                var listProductos = await _productoService.GetAll();
                return Ok(listProductos); ;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpGet("admin/{id}")]
        //Busco datos completos de un producto, solo admins
        public async Task<IActionResult> GetOneAdmin(int id)
        {

            try
            {
                var categoria = await _productoService.GetOneAdmin(id);

                return categoria != null ? Ok(categoria) : StatusCode(StatusCodes.Status500InternalServerError,
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
        //Agrego un Producto nuevo, solo admins y superAdmin tienen permiso
        public async Task<IActionResult> Add(ProductoAdminViewModel producto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _productoService.Add(producto) ? Ok(new Response
                    { Status = "Success", Message = "Se agrego el nuevo producto con exito!." }) : throw new Exception();
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
            }
            return NotFound("El producto ingresado no es valido.");
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPut("{id}")]
        //Edito un Producto cualquiera, solo admins y superAdmin tienen permiso
        public async Task<IActionResult> Edit([FromBody] ProductoAdminViewModel productoEditado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _productoService.Edit(productoEditado) ? Ok(new Response
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

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPost("softDelete/{id}")]
        //Se elimina logicamente un producto cualquiera admins y superAdmin tienen permiso
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                return await _productoService.SoftDelete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino logicamente la Categoria con exito!." }) : throw new Exception();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error o el id no existe, vuelva a intentarlo." });
            }
            //aca podria hacer un catch de una custom excepcion para detectar que el id no exite
            //return NotFound("El id ingresado no exite!");
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpPost("restore/{id}")]
        //Recuperacion de la eliminacion logica de un producto cualquiera, para admins y superAdmins
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                return await _productoService.Restore(id) ? Ok(new Response
                { Status = "Success", Message = "Se recupero logicamente el producto con exito!." }) : throw new Exception();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
            //Aca podria detectar si no se encontro el id con una Excepcion 
            //return NotFound("El id ingresado no exite!");
        }

        [Authorize(Roles = UserRoles.Admin + "," + UserRoles.SuperAdmin)]
        [HttpDelete("{id}")]
        //Borrado permanente de un producto cualquiera, solo admin y superAdmin
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                return await _productoService.Delete(id) ? Ok(new Response
                { Status = "Success", Message = "Se elimino permanentemente el producto con exito!." }) : throw new Exception();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
            //Aca podria detectar si no se encontro el id con una Excepcion 
            //return NotFound("El id ingresado no exite!");           
        }

        [Authorize(Roles = UserRoles.Manager)]
        [HttpPost("manager")]
        //Agrego un Producto nuevo para el bar asignado al manager, solo para managers
        public async Task<IActionResult> Add(ProductoManagerViewModel producto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _productoService.AddManager(producto) ? Ok(new Response
                    { Status = "Success", Message = "Se agrego el nuevo producto con exito!." }) : throw new Exception();
                }
                catch (Exception)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response
                    { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
                }
            }
            return NotFound("El producto ingresado no es valido.");
        }

        [Authorize(Roles = UserRoles.Manager)]
        [HttpPut("manager/{id}")]
        //Edito un Producto de un bar del manager, solo para managers
        public async Task<IActionResult> Edit([FromBody] ProductoManagerViewModel productoEditado)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    return await _productoService.EditManager(productoEditado) ? Ok(new Response
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

        [Authorize(Roles = UserRoles.Manager)]
        [HttpPost("softDelete/Manager")]
        //Se elimina logicamente un producto del bar perteneciente al manager , solo para managers
        public async Task<IActionResult> SoftDelete([FromBody]SoftDeleteManagerViewModel productoVm)
        {
            try
            {
                return await _productoService.SoftDeleteManager(productoVm) ? Ok(new Response
                { Status = "Success", Message = "Se borro logicamente el producto con exito!." }) : throw new Exception();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response
                { Status = "Error", Message = "Ocurrio un error, vuelva a intentarlo." });
            }
        }
    }
}
