using BarCrudApi.Models;

namespace BarCrudApi.ViewModels
{
    public class PedidoDetalleViewModel
    {
        public ProductoViewModel Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public PedidoDetalleViewModel(PedidoDetalle pedidoDetalle)
        {
            Producto = new ProductoViewModel(pedidoDetalle.Producto);
            Cantidad = pedidoDetalle.Cantidad;
            PrecioUnitario = pedidoDetalle.PrecioUnitario;
        }
    }
}
