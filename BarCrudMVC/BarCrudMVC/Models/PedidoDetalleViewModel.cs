namespace BarCrudMVC.Models
{
	public class PedidoDetalleViewModel
	{
		public ProductoViewModel Producto { get; set; }
		public int Cantidad { get; set; }
		public decimal PrecioUnitario { get; set; }
	}
}
