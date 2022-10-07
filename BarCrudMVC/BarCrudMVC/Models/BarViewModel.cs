using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
    public class BarViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una direccion")]
        public string? Direccion { get; set; }
        public string? Imagen { get; set; }

    }
}
