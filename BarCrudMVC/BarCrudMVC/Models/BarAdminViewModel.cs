using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
    public class BarAdminViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una direccion")]
        public string? Direccion { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? ManagerDni { get; set; }
        public string? Imagen { get; set; }
        public PersonaViewModel? manager { get; set; }
    }
}
