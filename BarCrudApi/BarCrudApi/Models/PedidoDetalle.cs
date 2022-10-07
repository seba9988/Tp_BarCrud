using System;
using System.Collections.Generic;

namespace BarCrudApi.Models
{
    public partial class PedidoDetalle
    {
        public int PedidoId { get; set; }
        public int ProductoId { get; set; }
        public decimal PrecioUnitario { get; set; }
        public short Cantidad { get; set; }

        public virtual Pedido Pedido { get; set; } = null!;
        public virtual Producto Producto { get; set; } = null!;
    }
}
