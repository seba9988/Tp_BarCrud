using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
    public class ProductoViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una Descripcion")]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "Se requiere un precio")]
        [RegularExpression(@"^(\d+).(\d{2})$", ErrorMessage = "El rango de precio debe estar entre 0,00 - 9999999999,99. ")]
        public decimal Precio { get; set; }
        [Required(ErrorMessage = "Se requiere un stock")]
        public int Stock { get; set; }
        public string? Imagen { get; set; }
        [Required(ErrorMessage = "Se requiere un id categoria")]
        public int CategoriaId { get; set; }
        public BarViewModel? Bar { get; set; }
        public CategoriaViewModel? Categoria { get; set; }

        public ProductoViewModel() { }
    }
}
