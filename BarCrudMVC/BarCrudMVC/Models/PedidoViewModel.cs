namespace BarCrudMVC.Models
{
	public class PedidoViewModel
	{
        public decimal Total { get; set; }
        public IList<PedidoDetalleViewModel> PedidoDetalles { get; set; }
    }
}
