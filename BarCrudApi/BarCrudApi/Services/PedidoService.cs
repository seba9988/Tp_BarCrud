using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;

namespace BarCrudApi.Services
{
    public class PedidoService: IPedidoService 
    {
        private readonly BarCrudContext _context;
        public PedidoService( BarCrudContext context) 
        { 
            _context = context;
        }
        //Busca el pedido sin finalizar de un usuario, junto con sus pedidos detalles y productos
        //si existe el pedido pendiente se actualiza los precios y verifica stock
        //si no hay stock de los productos se cancela el pedido
        public async Task<Pedido?> GetPedidoPendiente(string userId) 
        {
            try 
            {
                //Busco la persona con el userId
                var cliente = await _context.Personas
                    .Where(p => p.IdUsuario == userId)
                    .FirstAsync();
                //si existe busco su pedido sin finalizar si lo tiene
                if (cliente != null)
                {
                    var pedido = await _context.Pedidos
                        .Include(p => p.PedidoDetalles)
                        .ThenInclude(pd => pd.Producto)
                        .Where(p => p.ClienteDni == cliente.Dni
                        && p.FechaCompra == null)
                        .FirstAsync();
                    //Actualizo precios
                    if (pedido != null)
                        ActulizarPrecioTotal(pedido);
                    
                    return pedido;
                }
                //no existe el cliente
                return null;
            }
            catch (Exception) { return null; }        
        }
        //Busca historial de pedidos de un usuario
        public async Task<IList<PedidoHistorialViewModel>> GetHistorialUsuario(string userId)
             => await _context.Pedidos
            .Include(p => p.ClienteDniNavigation)
            .Where(p => p.FechaCompra != null
            && p.ClienteDniNavigation.IdUsuario == userId)
            .Select(p => new PedidoHistorialViewModel(p)).ToListAsync();
        //Busca todos pedidoDetalle de pedidos finalizados de un bar, con sus productos y bar
        public async Task<IList<PedidoDetalleBarViewModel>?> GetHistorialBar(int barId)
            => await _context.PedidoDetalles
            .Include(p => p.Producto)          
            .ThenInclude(p => p.Bar)
            .Include(p => p.Pedido)
            .Where(p => p.Producto.Bar.Id == barId
            && p.Pedido.FechaCompra != null)
            .Select(p => new PedidoDetalleBarViewModel(p)).ToListAsync();
        //Busca todos los datos de un pedido
        public async Task<PedidoViewModel> GetOnePedido(int pedidoId)
             => await _context.Pedidos
            .Include(p => p.PedidoDetalles)
            .ThenInclude(p => p.Producto)
            .Where(p => p.FechaCompra != null
            && p.Id == pedidoId)
            .Select(p => new PedidoViewModel(p)).FirstAsync();
        //Se agrega un producto al pedido sin finalizar de un usuario
        //si el usuariono tiene un pedido sin finalizar se crea uno nuevo
        //y se agrega el producto
        public async Task<bool> Add(CarritoAddViewModel model) 
        {
            try
            {
                //Busca el pedido pendiente de un usuario con su precio actualizado
                var pedidoPendiente = await GetPedidoPendiente(model.UserId);

                if(pedidoPendiente != null) 
                {
                    //Busco si este producto ya esta dentro del pedido
                    //o sino creo nuevo PedidoDetalle
                    var existeProducto = pedidoPendiente.PedidoDetalles
                            .Where(p => p.ProductoId == model.ProductoId)
                            .Select(p => p).FirstOrDefault();
                    //Si el producto ya esta en el pedido sumo cantidad si esta no supera el stock del producto
                    if (existeProducto != null)
                    {
                        //Si la cantidad actual del producto en pedidoDetalle + la nueva cantidad
                        //es menor o igual al stock, sumo cantidad y actualizo total
                        if (existeProducto.Cantidad + model.Cantidad <= existeProducto.Producto.Stock)
                        {
                            //Actualizo cantidad y total
                            existeProducto.Cantidad += model.Cantidad;
                            pedidoPendiente.Total += existeProducto.Cantidad * existeProducto.PrecioUnitario;
                            //guardo cambios
                            _context.SaveChanges();
                            return true;
                        }
                        //La cantidad que se quiere comprar supera el stock del producto
                        return false;
                    }
                    //Creo el nuevo pedidoDetalle si la cantidad es menor o igual al stock
                    return await CrearPedidoDetalle(model, pedidoPendiente) ? true : false;
                }
                //si no tiene un pedido pendiente creo uno
                var pedido = await CrearPedidoPendiente(model.UserId);
                //Creo el nuevo pedidoDetalle si la cantidad es menor o igual al stock
                return await CrearPedidoDetalle(model, pedido) ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Se remueve un producto del pedido pendiente de un usuario
        public async Task<bool> RemoveProducto(CarritoRemoveViewModel model)
        {
            //busco pedidoPendiente con precios actualizados
            var pedidoPendiente = await GetPedidoPendiente(model.UserId);
            
            if (pedidoPendiente != null)
            {
                //Si existe el pedido busco si este producto ya esta dentro del pedido
                var existeProducto = pedidoPendiente.PedidoDetalles
                       .Where(p => p.ProductoId == model.ProductoId)
                       .Select(p => p).First();
                //Si el producto ya esta en el pedido lo remuevo y actualizo total
                if (existeProducto != null)
                {
                    //Actualizo total del pedido
                    pedidoPendiente.Total -= existeProducto.PrecioUnitario * existeProducto.Cantidad;
                    //remuevo el pedidoDetalle 
                    _context.Remove(existeProducto);
                    await _context.SaveChangesAsync();
                    return true;
                }
                //no existe el producto que se quiere remover
                return false;
            }
            //el usuario no tiene pedido pendiente
            return false;
        }
        //Se remueve todos los productos del pedido y lo elimino 
        public async Task<bool> CancelarPedido(string userId)
        {
            try 
            {
                var pedidoPendiente = await GetPedidoPendiente(userId);
                //Si existe el pedido busco si este producto ya esta dentro del pedido
                if (pedidoPendiente != null)
                {
                    //Remuevo todos los productos en el pedido 
                    foreach (var pedidoDetalle in pedidoPendiente.PedidoDetalles)
                    {
                        _context.PedidoDetalles.Remove(pedidoDetalle);
                    }
                    //Cancelo pedido
                    _context.Pedidos.Remove(pedidoPendiente);
                    await _context.SaveChangesAsync();
                    return true;
                }
                //usuario no tiene pedido pendiente
                return false;
            }
            catch (Exception) { return false; }
          
        }

        //Se efctua la compra del pedido de un usuario
        //si tiene almenos un pedidoDetelle
        public async Task<bool> Compra(string userId) 
        {
            //busco pedido pendiente del usuario con precios actualizados
            var pedido = await GetPedidoPendiente(userId);
            //compuebo que exista el pedido pendiente y tenga pedidoDetalle
            if (pedido != null && pedido.PedidoDetalles != null)
            {
                //Verifico si hay stock para realizar la compra
                if(VerificarStock(pedido))
                {
                    var transaction = _context.Database.BeginTransaction();
                    //Resto stock a los productos
                    foreach(var pedidoDetalle in pedido.PedidoDetalles)
                    {
                        pedidoDetalle.Producto.Stock -= pedidoDetalle.Cantidad;
                    }

                    //finalizo el pedido seteandole fechaCompra
                     pedido.FechaCompra = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                //No hay stock de algun producto
                return false;
            }
            //No se puede finalizar un pedido si no tiene pedidoDetalle
            return false;
        }
        //se actualiza el precio total y unitario del pedido, en caso de que sus productos cambien de precio
        private void ActulizarPrecioTotal(Pedido pedido) 
        {
            pedido.Total = 0;
            //Actualizo precio unitario en pedidoDetalle y sumo al total

            foreach (var pedidoDetalle in pedido.PedidoDetalles) 
            {
                //Actualizo precio unitario
                pedidoDetalle.PrecioUnitario = pedidoDetalle.Producto.Precio;
                //Sumo al total por cada producto
                pedido.Total += pedidoDetalle.Cantidad * pedidoDetalle.PrecioUnitario;
            }
        }
        //Creo y guardo un pedido pendiete para un usuario
        private async Task<Pedido> CrearPedidoPendiente(string userId)
        {
            //Busco persona del usuario
            var cliente = await _context.Personas
                .Where(p => p.IdUsuario == userId)
                .Select(p => p).FirstAsync();
            //Creo nuevo pedido pendiente con sus datos
            Pedido pedidoNuevo = new Pedido
            {
                ClienteDni = cliente.Dni,
                Total = 0,
                FechaCompra = null
            };
            //guardo pedido
            await _context.AddAsync(pedidoNuevo);
            await _context.SaveChangesAsync();
            return pedidoNuevo;
        }
        //Creo y guardo el pedidoDetalle de un pedido
        private async Task<bool> CrearPedidoDetalle(CarritoAddViewModel model, Pedido pedido) 
        {
            //Busco producto a agregar
            var productoAgregar = await _context.Productos
                .Where(p => p.Id == model.ProductoId)
                .Select(p => p).FirstAsync();
            //verifico que no se quiera comprar mayor cantidad al stock del producto
            if(model.Cantidad <= productoAgregar.Stock) 
            {
                //Creo nuevo pedidoDetalle con sus datos
                var pedidoDetalle = new PedidoDetalle()
                {
                    PedidoId = pedido.Id,
                    ProductoId = productoAgregar.Id,
                    Cantidad = model.Cantidad,
                    PrecioUnitario = productoAgregar.Precio
                };
                //guardo pedidoDetalle
                await _context.AddAsync(pedidoDetalle);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        //Verifico el stock de todos los productos en el pedido
        public bool VerificarStock(Pedido pedido)
        {
            //Recorro pedidosDetalles
            foreach(var pedidoDetalle in pedido.PedidoDetalles) 
            {
                //Compruebo stock del producto con la cantidad en el pedidoDetalle o si el producto tiene baja logica
                if (pedidoDetalle.Producto.Stock < pedidoDetalle.Cantidad 
                    || pedidoDetalle.Producto.FechaBaja != null)
                    return false;
            }
            //si todos dan true significa que hay stock
            return true;
        }
    }
}
