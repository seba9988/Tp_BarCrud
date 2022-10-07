using BarCrudApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
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

        public PersonaViewModel() { }
        public PersonaViewModel(Persona persona) 
        {
            Nombre = persona.Nombre;
            Apellido = persona.Apellido;
            Dni = persona.Dni;
            IdUsuario = persona.IdUsuario;
        }
    }
}
