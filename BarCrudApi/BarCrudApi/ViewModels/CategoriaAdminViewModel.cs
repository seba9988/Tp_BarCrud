using BarCrudApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class CategoriaAdminViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una Descripcion")]
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public DateTime? FechaBaja { get; set; }

        public CategoriaAdminViewModel() { }
        public CategoriaAdminViewModel(Categoria categoria)
        {
            Id = categoria.Id;
            Nombre = categoria.Nombre;
            Descripcion = categoria.Descripcion;
            Imagen = categoria.Imagen;
            FechaBaja = categoria.FechaBaja;
        }
    }
}
