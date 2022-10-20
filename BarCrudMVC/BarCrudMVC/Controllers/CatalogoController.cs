using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarCrudMVC.Controllers
{
	public class CatalogoController : Controller
	{
        private readonly ICategoriaService _categoriaService;
        private readonly IProductoService _productoService;
        public CatalogoController(ICategoriaService categoriaService, IProductoService productoService) 
        { 
            _categoriaService = categoriaService;
            _productoService = productoService; 
        }
        [AllowAnonymous]
        //Busco y muestro los productos y categorias, para usuarios sin loguear o logueados
        public async Task<IActionResult> Index()
        {
            try
            {
                var categorias = await _categoriaService.GetAllSinBaja();
                var productos = await _productoService.GetAllSinBaja();
                ViewBag.Productos = productos;
                ViewBag.Categorias = categorias;
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
    }
}
