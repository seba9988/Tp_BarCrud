using BarCrudApi.Models;

namespace BarCrudApi.ViewModels
{
    public class PedidoViewModel
    {
        public decimal? Total { get; set; }
        public IList<PedidoDetalleViewModel> PedidoDetalles { get; set; }
        public PedidoViewModel(Pedido pedido)
        {
            if (pedido != null)
            {
                Total = pedido.Total;
                PedidoDetalles = new List<PedidoDetalleViewModel>();
                foreach (var pedidoDetalle in pedido.PedidoDetalles)
                {
                    PedidoDetalles.Add(new PedidoDetalleViewModel(pedidoDetalle));
                }
            }          
        }
    }
}
