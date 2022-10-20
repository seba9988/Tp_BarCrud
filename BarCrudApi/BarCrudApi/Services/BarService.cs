using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BarCrudApi.Services
{
    public class BarService : IBarService
    {
        private readonly BarCrudContext _context;
        private readonly IUserManagementService _userManagementService;
        public BarService(BarCrudContext context, IUserManagementService userManagementService) 
        {
            _context = context;
            _userManagementService = userManagementService;
        }
        //Busca todos los bares, esten con baja logica o no
        public async Task<IList<BarAdminViewModel>> GetAll()
            => await _context.Bares.Select(b => new BarAdminViewModel(b)).ToListAsync();
        //Busca los datos de un bar
        public async Task<BarViewModel?> GetOne(int id)
        {
            try
            {
                var bar = await _context.Bares.Where(e => e.Id == id)
                    .FirstAsync();
                //Agrego datos de bar al viewModel
                if (bar != null)
                {
                    var barVM = new BarViewModel(bar);
 
                    return barVM;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Busco datos completos de un bar incluyendo su manager, solo para admins y superAdmins
        public async Task<BarAdminViewModel?> GetOneAdmin(int id) 
        {
            try
            {
                var bar = await _context.Bares.Where(e => e.Id == id)
                    .Include(b => b.ManagerDniNavigation)
                    .FirstAsync();
                //Agrego datos de bar al viewModel
                if (bar != null)
                {
                    var barVM = new BarAdminViewModel(bar);

                    if (bar.ManagerDniNavigation != null)
                        barVM.manager = new PersonaViewModel(bar.ManagerDniNavigation);
                    return barVM;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Busca todos los bares que no esten de baja, fechaBaja=null
        public async Task<IList<BarViewModel>> GetAllSinBaja()
             => await _context.Bares.Where(b => b.FechaBaja == null)
            .Select(b => new BarViewModel(b)).ToListAsync();
        //Busca todos los bares que no esten de baja y no tenga manager, fechaBaja=null       
        public async Task<IList<BarViewModel>> GetAllSinManager()
             => await _context.Bares.Where(b => b.FechaBaja == null && b.ManagerDni == null)
            .Select(b => new BarViewModel(b)).ToListAsync();
        //Busca todos los dato del bar perteneciente al id usuario de un manager, solo para managers
        public async Task<BarViewModel> GetAllManager(string id)
        {
            //Busco el manager
            var manager = await _context.Personas.Where(p => p.IdUsuario == id).FirstAsync();
            if(manager != null)
            {
                //Busco bares sin baja del manager
                var bar = await _context.Bares
                    .Where(b => b.ManagerDni == manager.Dni && b.FechaBaja == null)
                    .Select( b =>new BarViewModel(b))
                    .FirstAsync();
                return bar;
            }
            //si no existe el usuario devuelvo empty           
            return null;
        }

        //Agrega un bar nuevo si este es valido para agregar,false si no se agrego, true si se agrega
        //Tambien se le asigna un manager, verificar si este manager ya pertenece a otro bar
        public async Task<bool> Add(BarAdminViewModel barVM)
        {
            try
            {   
                //verifico si se ingreso un dni
                if(barVM.ManagerDni != null) 
                {
                    //busco si el manager ingresado existe
                    var managerConBar = await _context.Personas
                        .Include(p => p.Bares)
                        .Where(p => p.Dni == barVM.ManagerDni)
                        .FirstOrDefaultAsync();
                    //si el manager no existe o ya tiene asignado un bar devuelvo false
                    if (managerConBar == null || managerConBar.Bares != null)
                        return false;                    
                }
                //Si no se selecciono ningun manager o el manager no tienen ningun bar asignado se agrega el bar
                var barNuevo = new Bar(barVM);
                _context.Bares.Add(barNuevo);
                await _context.SaveChangesAsync();
                return true;

            }
            catch (Exception) { return false; }
        }
        //Edita una un bar si este es valido para editar,false si no se edito, true si se edita
        public async Task<bool> Edit(BarAdminViewModel barEditado)
        {
            var barActual = _context.Bares.Find(barEditado.Id);
            if (barActual != null)
                try
                {
                    barActual.Nombre = barEditado.Nombre;
                    barActual.Direccion = barEditado.Direccion;
                    barActual.ManagerDni = barEditado.ManagerDni;
                    barActual.Imagen = barEditado.Imagen;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            return false;
        }

        //Elimina un bar en cascada,false si no se elimino, true si se elimina
        //Esto implica borrar los pedidos y pedidos detalle
        public async Task<bool> Delete(int id)
        {
            //busco bar con sus productos
            var barEliminar = await _context.Bares
                .Include(b => b.Productos)
                .Where(b => b.Id == id)
                .FirstAsync();
            //Definimos la logica de eliminacion, si existe se borra
            if (barEliminar != null)
            {
                try
                {
                    //Inicio transaccion
                    var transaction = _context.Database.BeginTransaction();
                    //Elimino productos
                    foreach(var producto in barEliminar.Productos) 
                    {
                         _context.Productos.Remove(producto);
                    }
                    //Elimino el bar
                    _context.Bares.Remove(barEliminar);                                       
                    //guardo cambios
                    await _context.SaveChangesAsync();
                    //Termino transaccion y devuelvo true
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
        //Elimina un bar logicamente si este es valido para eliminar,false si no se elimino, true si se elimina
        //Tambien se da baja logica a sus productos que no tengan baja logica,
        //Haciendo esto los productos que se dieron de baja con anterioridad a la baja logica tienen una fecha baja diferente
        //de esta forma cuando recupero el bar con restore
        //solo voy a recuperar los productos con la misma fecha baja que el bar
        public async Task<bool> SoftDelete(int id)
        {
            var barActual = _context.Bares.Find(id);
            
            //Si existe e bar lo elimino logicamente 
            if (barActual != null)
                try
                {
                    //Inicio transaccion
                    var transaction = _context.Database.BeginTransaction();
                    //Doy baja logica al bar
                    barActual.FechaBaja = DateTime.UtcNow;
                    //Busco productos del bar sin baja
                    var productosBar = await _context.Productos
                        .Where(p => p.BarId == barActual.Id && p.FechaBaja == null).ToListAsync();
                    //Hago baja logica de productos del bar si la lista no es nula
                    if(productosBar != null) 
                    {
                        foreach(var producto in productosBar)
                        {
                            producto.FechaBaja = barActual.FechaBaja;
                        }                                                            
                    }
                    //Guardo cambios
                    await _context.SaveChangesAsync();
                    //Termino transaccion
                    await transaction.CommitAsync();
                    return true;                   
                }
                catch (Exception)
                {
                    return false;
                }
            return false;
        }
        //Recupera un bar eliminada logicamente si este es valido para recuperar
        //Tambien recupero los productos con la fecha baja igual a la del bar
        //para recuperar solo los productos borrados logicamente con el softDelete
        //para evitar inconsistencias en el softDelete de producto se prohibe recuperar productos si
        //su bar perteneciente esta con baja o su categoria esta de baja logica
        public async Task<bool> Restore(int id) 
        {
            var barARecuperar = _context.Bares.Find(id);
            //si existe el bar lo recupero y busco sus productos con mayor fecha baja
            if (barARecuperar != null)
            {
                try
                {
                    //Busco productos con la misma fecha baja que el bar
                    var productosBar = await _context.Productos.Where(p => p.FechaBaja == barARecuperar.FechaBaja)
                        .ToListAsync();
                    // Inicio transaccion
                    var transaction = _context.Database.BeginTransaction();
                    //Recupero el bar
                    barARecuperar.FechaBaja = null;
                    //Si la lista productos no es nula recupero los recupero
                    if(productosBar != null) 
                    {
                        foreach(var producto in productosBar)
                        {
                            producto.FechaBaja = null;
                        }
                    }
                    //Guardo cambios
                    await _context.SaveChangesAsync();
                    //Termino transaccion
                    await transaction.CommitAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;       
        }      
    }
}
