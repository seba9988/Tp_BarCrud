using BarCrudMVC.Models;
using BarCrudMVC.Services;
using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarCrudMVC.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IProductoService _productoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IBarService _barService;
        public ProductoController(IProductoService productoService, 
            ICategoriaService categoryService,
            IBarService barService)
        {
            _productoService = productoService;
            _categoriaService = categoryService;
            _barService = barService;
        }
        [AllowAnonymous]
        //Busco y muestro todos los productos sin baja, para usuarios sin loguear o logueados
        public async Task<IActionResult> Index()
        {
            try
            {
                var productos = await _productoService.GetAllSinBaja();

                return View(productos);
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        [AllowAnonymous]
        //Busco y muestro los productos de una categoria
        public async Task<IActionResult> ProductosPorCategoria(string nombre,int id)
        {
            try
            {
                var productos = await _productoService.GetAllByCategoria(id);
                ViewBag.Productos = productos;
                //buardo nombre de la categoria en un viewbag
                if(nombre != null)
                    ViewBag.Nombre = nombre;

                return View("Index");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        [AllowAnonymous]
        //Busco y muestro los productos de un bar
        public async Task<IActionResult> ProductosPorBar(string nombre, int id)
        {
            try
            {
                var productos = await _productoService.GetAllByBar(id);
                ViewBag.Productos = productos;
                //buardo nombre del bar
                if (nombre != null)
                    ViewBag.Nombre = nombre;

                return View("Index");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se busca y muestra datos del producto seleccionado
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> VerProducto(int id)
        {
            try
            {
                //busco datos de la categoria
                var producto = await _productoService.GetOne(id);
                if (producto != null)
                {
                    return View(producto);
                }
                ViewBag.Fallo = "No se encontraron datos del producto.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }

        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        //Busco y traigo todas los productos con o sin baja, solo admins y superAdmins
        public async Task<IActionResult> ProductoAdmin()
        {
            try
            {
                var categorias = await _productoService.GetAll();
                return View(categorias);
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se pasa a la view Add
        //Busco las categorias para mostrar como opcion en el add
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        public async Task<IActionResult> AddView()
        {
            try
            {
                //Busco listado de categorias disponibles sin baja y lo asigno a un viewBag
                var categorias = await _categoriaService.GetAllSinBaja();
                ViewBag.Categorias = categorias;
                //busco listado de bares y lo asigno a un viewBag
                var bares = await _barService.GetAllSinBaja();
                ViewBag.Bares = bares;
                return View("Add");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion del producto
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Add(ProductoAdminViewModel producto)
        {
            try
            {
                if (await _productoService.Add(producto))
                {
                    ViewBag.Exito = "Se edito con exito el producto!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Agregacion del producto.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busca los datos completos del producto a editar, listado de categorias y bares, luego muestra la vista editar
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        public async Task<IActionResult> EditView(int id)
        {
            try
            {
                var producto = await _productoService.GetOneAdmin(id);
                if (producto != null) 
                {
                    //Busco listado de categorias disponibles sin baja
                    var categorias = await _categoriaService.GetAllSinBaja();
                    ViewBag.Categorias = categorias;
                    //busco listado de bares disponibles sin baja
                    var bares = await _barService.GetAllSinBaja();
                    ViewBag.Bares = bares;
                    return View("Edit", producto);
                }                  
                ViewBag.Fallo = "Fallo la busqueda del producto.";
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
        public async Task<IActionResult> Edit(ProductoAdminViewModel producto)
        {           
            try
            {
                if (await _productoService.Edit(producto))
                {
                    ViewBag.Exito = "Se edito con exito el producto!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Edicion del producto.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        
        //Se busca y muestra datos del producto seleccionado
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpGet]
        public async Task<IActionResult> VerProductoAdmin(int id)
        {
            try
            {
                //busco datos de la categoria
                var producto = await _productoService.GetOneAdmin(id);
                if (producto != null)
                {
                    return View(producto);
                }
                ViewBag.Fallo = "No se encontraron datos del producto.";
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
                if (await _productoService.SoftDelete(id))
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
        //Lo que implica que tambien se va a borrar stocks, pedidos y detalle pedido de este producto
        [Authorize(Roles = ("SuperAdmin") + "," + ("Admin"))]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja permanente
                if (await _productoService.Delete(id))
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
                if (await _productoService.Restore(id))
                {
                    ViewBag.Exito = "Se recupero el producto con extio!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "No se pudo recuperar el producto.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }

        [Authorize(Roles = "Manager")]
        [HttpGet]
        //Con un id bar busco los productos sin baja de ese bar y devuelvo la vista manager
        public async Task<IActionResult> ProductosBarManager(string nombreBar, int id)
        {
            try
            {
                var productos = await _productoService.GetAllByBar(id);
                //Guardo nombre del bar en el viewbag
                if (nombreBar != null)
                    ViewBag.Nombre = nombreBar;

                return View("ProductosBarManager", productos);
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }

        //Se pasa a la view Add
        //Busco las categorias para mostrar como opcion en el add
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AddManagerView()
        {
            try
            {
                //Busco listado de categorias disponibles sin baja y lo asigno a un viewBag
                var categorias = await _categoriaService.GetAllSinBaja();
                ViewBag.Categorias = categorias;
                //busco listado de bares y lo asigno a un viewBag
                var bares = await _barService.GetAllSinBaja();
                ViewBag.Bares = bares;
                return View("AddManager");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion del producto
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public async Task<IActionResult> AddManager(ProductoManagerViewModel producto)
        {
            try
            {
                if (await _productoService.Add(producto))
                {
                    ViewBag.Exito = "Se edito con exito el producto!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Agregacion del producto.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //busca los datos completos del producto a editar, listado de categorias y luego muestra la vista editar para manager
        [Authorize(Roles = ("Manager") )]
        public async Task<IActionResult> EditManagerView(int id)
        {
            try
            {
                var producto = await _productoService.GetOne(id);
                if (producto != null)
                {
                    //Busco listado de categorias disponibles sin baja
                    var categorias = await _categoriaService.GetAllSinBaja();
                    ViewBag.Categorias = categorias;
                    return View("EditManager", producto);
                }
                ViewBag.Fallo = "Fallo la busqueda del producto.";
                return View("EditManager");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la edicion del producto del manager
        [Authorize(Roles = ("Manager"))]
        [HttpPost]
        public async Task<IActionResult> EditManager(ProductoManagerViewModel producto)
        {
            try
            {
                if (await _productoService.Edit(producto))
                {
                    ViewBag.Exito = "Se edito con exito el producto!.";
                    return View("AccionResult");
                }
                ViewBag.Fallo = "Fallo la Edicion del producto.";
                return View("AccionResult");
            }
            catch (Exception)
            {
                ViewBag.Fallo = "Ocurrio un error.";
                return View("AccionResult");
            }
        }
        //Se efectua la baja logica
        [Authorize(Roles = ("Manager") )]
        [HttpPost]
        public async Task<IActionResult> SoftDeleteManager(int id)
        {
            try
            {
                //Compruebo si se efectuo exitosamente la baja logica
                if (await _productoService.SoftDeleteManager(id))
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

    }
}
