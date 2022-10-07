namespace BarCrudMVC.Models
{
	public class PedidoDetalleBarViewModel
	{
        public ProductoViewModel Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public DateTime? FechaCompra { get; set; }
    }
}
