using BarCrudApi.Models;

namespace BarCrudApi.ViewModels
{
    public class PedidoHistorialViewModel
    {
        public decimal Total { get; set; }
        public int Id { get; set; }
        public DateTime? FechaCompra { get; set; }
        public string NombreUsuario { get; set; }

        public PedidoHistorialViewModel(Pedido pedido) 
        {
            Total = pedido.Total;
            Id = pedido.Id;
            FechaCompra = pedido.FechaCompra;
            NombreUsuario = pedido.ClienteDniNavigation.Nombre;           
        }
    }
}
