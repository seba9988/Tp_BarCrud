using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarCrudApi.Services
{
    public class ProductoService:IProductoService
    {
        private BarCrudContext _context;

        public ProductoService(BarCrudContext context)
        {
            _context = context;
        }
        //Busca todos los productos con sus bares, esten con baja logica o no
        public async Task<IList<ProductoAdminViewModel>> GetAll()
            => await _context.Productos.Include(p => p.Bar)
            .Select(p => new ProductoAdminViewModel(p)).ToListAsync();

        //Busca todos los productos que no esten de baja Y tengan stock, fechaBaja=null
        public async Task<IList<ProductoViewModel>> GetAllSinBaja()
            => await _context.Productos.Where(p => p.FechaBaja == null && p.Stock > 0)
            .Include(p => p.Bar)
            .Select(p => new ProductoViewModel(p)).ToListAsync();
        //Busca todos los productos de una categoria que no esten de baja y tengan stock, fechaBaja=null
        public async Task<IList<ProductoViewModel>> GetAllByCategoria(int id)
            => await _context.Productos
            .Include(p => p.Bar)
            .Where(p => p.FechaBaja == null
            && p.CategoriaId == id && p.Stock > 0)
            .Select(p => new ProductoViewModel(p)).ToListAsync();
        //Busca todos los productos de un bar que no esten de baja y tengan stock
        public async Task<IList<ProductoViewModel>> GetAllByBar(int id)
            => await _context.Productos
            .Include(p => p.Bar)
            .Where(p => p.FechaBaja == null
            && p.BarId == id && p.Stock > 0)
            .Select(p => new ProductoViewModel(p)).ToListAsync();

        //Busco un producto con un id, incluyendo su bar
        //solo se lo trae si no tiene baja logica
        public async Task<ProductoViewModel?> GetOne(int id)
        {
            try
            {
                var producto = await _context.Productos
                    .Include(b => b.Categoria )
                    .Include(b => b.Bar)
                    .Where( p=> p.Id == id &&  p.FechaBaja == null)
                    .Select(p => new ProductoViewModel(p)).FirstAsync();

                return producto;
            }
            catch (Exception) { return null; }
        }
        //Busco un producto con un id, incluyendo su categoria y bar
        // se lo trae tenga o no baja logica
        public async Task<ProductoAdminViewModel?> GetOneAdmin(int id)
        {
            try
            {
                var producto = await _context.Productos.Include(b => b.Bar)
                    .Include(p => p.Categoria)
                    .Where(p => p.Id == id)
                    .Select(p => new ProductoAdminViewModel(p)).FirstAsync();

                return producto;
            }
            catch (Exception) { return null; }
        }

        //Agrega un producto nuevo si este es valido para guardar,false si no se agrego, true si se agrega
        public async Task<bool> Add(ProductoAdminViewModel productoVM)
        {
            try
            {
                var productoNuevo = new Producto(productoVM);
                _context.Productos.Add(productoNuevo);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) { return false; }
        }
        //Edita un producto si este es valido para editar,false si no se edito, true si se edita
        public async Task<bool> Edit(ProductoAdminViewModel productoEditado)
        {
            var productoActual = await _context.Productos.FindAsync(productoEditado.Id);
            if (productoActual != null)
                try
                {
                    productoActual.Nombre = productoEditado.Nombre;
                    productoActual.Imagen = productoEditado.Imagen;
                    productoActual.Precio = productoEditado.Precio;
                    productoActual.Descripcion = productoEditado.Descripcion;
                    productoActual.CategoriaId = productoEditado.CategoriaId;
                    productoActual.Stock = productoEditado.Stock;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            return false;
        }
 
        //Elimina un producto en cascada si este es valido para eliminar, false si no se elimino, true si se elimina
        //esto implica que se va a borrar  pedidos y detalle pedido de este producto en cascada
        public async Task<bool> Delete(int id)
        {
            var productoEliminar = await _context.Productos.FindAsync(id);
            //Definimos la logica de eliminacion, si existe se borra
            if (productoEliminar != null)
            {
                try
                {
                    _context.Productos.Remove(productoEliminar);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        //Elimina un producto logicamente si este es valido para eliminar
        //false si no se elimino, true si se elimina
        public async Task<bool> SoftDelete(int id)
        {
            var productoActual = _context.Productos.Find(id);

            //Si existe el producto y no esta ya con baja se elimina logicamente 
            if (productoActual != null && productoActual.FechaBaja == null)
                try
                {
                    productoActual.FechaBaja = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            //aca podria tirar una custom Exception para informar que el id no existe
            return false;
        }
        //Recupera un producto eliminada logicamente si esta es valida para recuperar
        //se prohibe restore si la categoria o bar del producto estan de baja
        public async Task<bool> Restore(int id)
        {
            var productoARecuperar = await _context.Productos
                .Include(p => p.Bar)
                .Include(p => p.Categoria)
                .Where(p => p.Categoria.FechaBaja == null 
                && p.Bar.FechaBaja == null && p.Id == id)
                .FirstAsync();

            if (productoARecuperar != null)
                try
                {
                    //Recupero el producto
                    productoARecuperar.FechaBaja = null;

                    await _context.SaveChangesAsync();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            //false si no se encontro producto o su categoria/Bar tienen baja
            return false;          
        }


        //Agrega un producto al bar del manager si el manager/bar no tienen baja
        //false si no se edito, true si se edita
        public async Task<bool> AddManager(ProductoManagerViewModel productoVM)
        {
            try
            {
                //Busco el bar con su su persona manager si esta corresponde al idUsuario indicato en el viewModel
                //y ambos no estan con baja
                var barPersonaManager = await _context.Bares.Include(b => b.ManagerDniNavigation)
                    .Where(b => b.ManagerDniNavigation.IdUsuario == productoVM.ManagerId 
                    && b.FechaBaja == null 
                    && b.ManagerDniNavigation.FechaBaja == null)
                    .FirstAsync();
                //Si existe el manager, tiene un bar asignado y cumple condiciones agrego producto
                if (barPersonaManager.ManagerDniNavigation != null)
                {
                    //instancio producto
                    var productoAgregar = new Producto(productoVM);
                    //Agrego bar id al producto 
                    productoAgregar.BarId = barPersonaManager.Id;

                    await _context.AddAsync(productoAgregar);
                    //guardo cambios
                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        //Edita un producto si este pertenece al bar del manager y si el manager/bar no tienen baja
        //false si no se edito, true si se edita
        public async Task<bool> EditManager(ProductoManagerViewModel productoEditado) 
        {
            try
            {
                //Busco si exite el producto y cumple las condiciones de manager
                var productoActual = await GetProductoManager(productoEditado);

                if (productoActual != null)
                {
                    //edito el producto
                    productoActual.Nombre = productoEditado.Nombre;
                    productoActual.Imagen = productoEditado.Imagen;
                    productoActual.Descripcion = productoEditado.Descripcion;
                    productoActual.CategoriaId = productoEditado.CategoriaId;
                    productoActual.Precio = productoEditado.Precio;
                    productoActual.Stock = productoEditado.Stock;
                    //guardo cambios
                    await _context.SaveChangesAsync();

                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Elimina un producto logicamente si este pertenece al bar del manager
        //false si no se elimino, true si se elimina
        public async Task<bool> SoftDeleteManager(SoftDeleteManagerViewModel managerBarId) 
        {
            try
            {
                var productVM = new ProductoManagerViewModel
                {
                    Id = managerBarId.ProductoId,
                    ManagerId = managerBarId.ManagerId
                };
                //Busco si exite el producto y cumple las condiciones de manager
                var productoABorrar = await GetProductoManager(productVM);
                //si existe y cumple condiciones efectuo la baja logica
                if (productoABorrar != null)
                {
                    //baja logica
                    productoABorrar.FechaBaja = DateTime.UtcNow;
                    //Guardo cambios
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //Busca y trae un producto si este no tiene baja y pertenece al bar del manager ingresado en el viewModel,
        //el manager y bar no deben estar de baja.
        //Se recibe un ProductoManagerViewModel que contiene id del producto y del usuarioManager
        private async Task<Producto?> GetProductoManager(ProductoManagerViewModel productoVM) 
        {
            // busco datos del manager para saber para cual bar trabaja y si no tiene baja
            var personaManager = await _context.Personas
                .Where(p => p.IdUsuario == productoVM.ManagerId && p.FechaBaja == null)
                .FirstAsync();
            if(personaManager != null) 
            {
                //Busco si el producto no tiene baja y pertenece al bar asociado al dni del manager
                var producto = await _context.Productos
                    .Include(p => p.Bar)
                    .ThenInclude(b => b.ManagerDniNavigation)
                    .Where(p => p.Bar.ManagerDniNavigation.Dni == personaManager.Dni
                        && p.Id == productoVM.Id  
                        && p.Bar.FechaBaja == null
                        && p.FechaBaja == null)
                    .FirstAsync();

                return producto;
            }
            //Devuelvo null si el manager no existe o tiene baja
            return null;
        }
    }
}
