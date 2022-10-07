using BarCrudMVC.Models;

namespace BarCrudMVC.Services.Interfaces
{
    public interface IPedidoService
    {
        //Busca el pedido sin finalizar de un usuario
        Task<PedidoViewModel?> GetPedidoPendiente();
        //Busca historial de pedidos de un usuario
        Task<IList<PedidoHistorialViewModel>> GetHistorialUsuario();
        //Busca todos pedidoDetalle de pedidos finalizados de un bar, con sus productos y bar
        Task<IList<PedidoDetalleBarViewModel>?> GetHistorialBar(int barId);

        //Busca todos los datos de un pedido
        Task<PedidoViewModel> GetOnePedido(int pedidoId);
        //Se agrega un producto al pedido sin finalizar de un usuario
        //si el usuariono tiene un pedido sin finalizar se crea uno nuevo
        //y se agrega el producto
        Task<bool> Add(CarritoAddViewModel model);
        //Se remueve un producto del pedido sin finalizar de un usuario
        Task<bool> RemoveProducto(CarritoRemoveViewModel mode);
        //Se efctua la compra del pedido de un usuario
        //si tiene almenos un pedidoDetella
        Task<bool> Compra();
        //Vacio el pedido pendiente del cliente y lo elimino
        Task<bool> CancelarPedido();
    }
}
