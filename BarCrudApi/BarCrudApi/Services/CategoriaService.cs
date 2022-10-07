using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BarCrudApi.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly BarCrudContext _context;

        public CategoriaService(BarCrudContext context)
        {
            _context = context;
        }
        //Busca todas las categorias, esten con baja logica o no
        public async Task<IList<CategoriaAdminViewModel>> GetAll()
            => await _context.Categorias.Select(c => new CategoriaAdminViewModel(c)).ToListAsync();
        //Busco una categoria con un id
        public async Task<CategoriaAdminViewModel?> GetOne(CategoriaAdminViewModel categoriaVM)
        {
            try
            {
                var categoria = await _context.Categorias.FindAsync(categoriaVM.Id);
                //Agrego datos de categoria al viewModel
                if (categoria != null)
                {
                    categoriaVM.Descripcion = categoria.Descripcion;
                    categoriaVM.Imagen = categoria.Imagen;
                    categoriaVM.Nombre = categoria.Nombre;
                    categoriaVM.FechaBaja = categoria.FechaBaja;
                }
                return categoriaVM;
            }
            catch (Exception) { return null; }
        }

        //Busca todas las categorias con sus productos, esten con baja logica o no
        //falta modificar viewModel para que pueda tener un objeto producto
        public async Task<IList<CategoriaProdAdminViewModel>> GetAllCategoriasProductos()
            => await _context.Categorias.Include(c => c.Productos)
            .Select(c => new CategoriaProdAdminViewModel(c)).ToListAsync();
        //Busca todas las categorias que no esten de baja, fechaBaja=null
        public async Task<IList<CategoriaViewModel>> GetAllSinBaja()
            => await _context.Categorias.Where(c => c.FechaBaja == null)
            .Select(c => new CategoriaViewModel(c)).ToListAsync();

        //Agrega una categoria nueva si esta es valida para guardar,false si no se agrego, true si se agrega
        public async Task <bool> Add(CategoriaViewModel categoriaVM) 
        {
            try 
            {
                var categoriaNueva = new Categoria(categoriaVM);
                _context.Categorias.Add(categoriaNueva);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) {return false;}            
        }
        //Edita una categoria si esta es valida para editar,false si no se edito, true si se edita
        public async Task<bool> Edit(CategoriaViewModel categoriaEditada) 
        {
            var categoriaActual = _context.Categorias.Find(categoriaEditada.Id);
            if (categoriaActual != null)
                try
                {
                    categoriaActual.Nombre = categoriaEditada.Nombre;
                    categoriaActual.Imagen = categoriaEditada.Imagen;
                    categoriaActual.Descripcion = categoriaEditada.Descripcion;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            return false;
        }
        //Elimina una categoria permanentemente en cascada si esta es valida para eliminar,false si no se elimino, true si se elimina
        //Implica borrar productos, stocks, pedidos y detalle pedidos en cascada
        public async Task<bool> Delete(int id) 
        {
            var categoriaEliminar = await _context.Categorias.FindAsync(id);
            //Definimos la logica de eliminacion, si existe se borra
            if (categoriaEliminar != null) 
            {
                try 
                {
                    //Busco productos de la categoria 
                    var productos = await _context.Productos.Where(p => p.CategoriaId == categoriaEliminar.Id).ToListAsync();
                    //Inicio transaccion
                    var transaction = _context.Database.BeginTransaction();
                    if (productos != null)
                    {
                        foreach (var p in productos)
                        {
                            _context.Productos.Remove(p);
                        }
                    }
                    _context.Categorias.Remove(categoriaEliminar);
                    await _context.SaveChangesAsync();
                    //Cierro transaccion
                    transaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;       
        }

        //Elimina una categoria logicamente si esta es valida para eliminar
        //Tambien se elimina logicamente sus productos que no tengan ya baja
        //de esta forma los productos eliminados antes a la baja logica categoria 
        //tienen una fecha baja diferente, asi cuando use restore solo recupero los productos
        //eliminados en la baja logica
        public async Task<bool> SoftDelete(int id) 
        {
            var categoriaActual = _context.Categorias.Find(id);
            
            //Si existe la categoria la elimino  logicamente y a sus productos

            if (categoriaActual != null)
                try
                {
                    //Inicio transaccion
                    var transaction = _context.Database.BeginTransaction();
                    //Aplico baja logica
                    categoriaActual.FechaBaja = DateTime.UtcNow;
                    //busco productos que no tengan baja
                    var prodsCategoria = _context.Productos
                        .Where(p => p.CategoriaId == categoriaActual.Id && p.FechaBaja == null);
                    //Doy baja logica a productos de la categoria si es que tiene,
                    //probablemente deba hacerlo con un stored procedure para
                    //mejor rendimiento o buscar alguna forma mejor en entity
                    if (prodsCategoria != null)
                    {
                        foreach (Producto p in prodsCategoria)
                        {
                            p.FechaBaja = categoriaActual.FechaBaja;
                        }                      
                    }
                    //guardo cambios
                    await _context.SaveChangesAsync();
                    //Commmit si todo se modifica bien, sino rollback automaticamente
                    await transaction.CommitAsync();
                    //true si se elimino correctamente
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            return false;
        }
        //Recupera una categoria eliminada logicamente si esta es valida para recuperar
        //Tambien recupero los productos que se dieron de baja en la baja logica de categoria
        //para evitar inconsistencias se prohibe restore de productos si su categoria
        //o bar estan con baja
        public async Task<bool> Restore(int id) 
        {
            var categoriaARecuperar = _context.Categorias.Find(id);           
            //si existe la categoria la recupero
            if (categoriaARecuperar != null)
            {
                try
                {
                    //busco productos de la categoria eliminados en la misma fecha que categoria
                    var productosCategoria = await _context.Productos
                        .Where(p => p.CategoriaId == categoriaARecuperar.Id 
                        && p.FechaBaja == categoriaARecuperar.FechaBaja).ToListAsync();
                    //comienzo transaccion
                    var transaction = _context.Database.BeginTransaction();

                    //Recupero la categoria
                    categoriaARecuperar.FechaBaja = null;
                    //Recupero los productos con baja logica
                    if (productosCategoria != null)
                    {
                        foreach (Producto p in productosCategoria)
                        {
                            p.FechaBaja = null;
                        }                    
                    }
                    //Guardo cambios
                    await _context.SaveChangesAsync();
                    //Si todo sale bien se hace commit, sino automaticamente se hace rollBack segun la documentacion
                    await transaction.CommitAsync();
                    //Si se guardan los cambios correctamente devuelvo true
                    return true;                 
                }
                catch (Exception)
                {
                    return false;
                }
            }
            //aca podria tirar una custom Exception para avisar de que el id no se encontro y avisar al controlador
            return false;
        }
    }
}
