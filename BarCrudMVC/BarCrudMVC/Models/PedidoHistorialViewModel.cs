namespace BarCrudMVC.Models
{
	public class PedidoHistorialViewModel
	{
        public decimal Total { get; set; }
        public int Id { get; set; }
        public DateTime? FechaCompra { get; set; }
        public IList<PedidoDetalleViewModel> PedidoDetalles { get; set; }
    }
}
