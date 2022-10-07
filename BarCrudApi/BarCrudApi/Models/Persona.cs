using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace BarCrudApi.Models
{
    public partial class Persona
    {
        public Persona()
        {
            Bares = new HashSet<Bar>();
            Pedidos = new HashSet<Pedido>();
        }

        public string Dni { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string? IdUsuario { get; set; }
        public DateTime? FechaBaja { get; set; }

        public virtual ICollection<Bar> Bares { get; set; }
        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
