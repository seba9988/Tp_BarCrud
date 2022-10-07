using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
    public class PersonaViewModel
    {
        [Required(ErrorMessage = "Nombre is required")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Apellido is required")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Dni is required"), MinLengthAttribute(8), MaxLength(8)]
        [Range(0, Int64.MaxValue, ErrorMessage = "Dni should not contain characters")]
        public string Dni { get; set; }
        public string IdUsuario { get; set; }
    }
}
