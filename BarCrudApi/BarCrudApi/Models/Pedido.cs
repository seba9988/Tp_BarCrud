using System;
using System.Collections.Generic;

namespace BarCrudApi.Models
{
    public partial class Pedido
    {
        public Pedido()
        {
            PedidoDetalles = new HashSet<PedidoDetalle>();
        }

        public int Id { get; set; }
        public string ClienteDni { get; set; } = null!;
        public DateTime? FechaCompra { get; set; }
        public decimal Total { get; set; }

        public virtual Persona ClienteDniNavigation { get; set; } = null!;
        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; }
    }
}
