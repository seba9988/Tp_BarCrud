using BarCrudApi.ViewModels;
using System;
using System.Collections.Generic;

namespace BarCrudApi.Models
{
    public partial class Categoria
    {
        //constructores
        public Categoria()
        {
            Productos = new HashSet<Producto>();
        }
        public Categoria(CategoriaViewModel categoriaVM)
        {
            Nombre = categoriaVM.Nombre;
            Descripcion = categoriaVM.Descripcion;
            Imagen = categoriaVM.Imagen;
        }

        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public DateTime? FechaBaja { get; set; }

        public virtual ICollection<Producto> Productos { get; set; }
    }
}
