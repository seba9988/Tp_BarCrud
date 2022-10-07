using BarCrudApi.Models;

namespace BarCrudApi.ViewModels
{
    //Se usa para mostrar el listado de ventas que hizo un bar, fecha, cual producto
    // a que precio en ese momento, cantidad.
    public class PedidoDetalleBarViewModel
    {
        public ProductoViewModel Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime? FechaCompra { get; set; }
        public PedidoDetalleBarViewModel(PedidoDetalle pedidoDetalle) 
        {
            Producto = new ProductoViewModel(pedidoDetalle.Producto);
            Cantidad = pedidoDetalle.Cantidad;
            PrecioUnitario = pedidoDetalle.PrecioUnitario;
            FechaCompra = pedidoDetalle.Pedido.FechaCompra;
        }

    }
}
