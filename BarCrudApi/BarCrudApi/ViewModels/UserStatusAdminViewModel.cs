using BarCrudApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BarCrudApi.ViewModels
{
    public class UserStatusAdminViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "User Name is required"), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nombre is required")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Apellido is required")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "Dni is required"), MinLengthAttribute(8), MaxLength(8)]
        [Range(0, Int64.MaxValue, ErrorMessage = "Dni should not contain characters")]
        public string Dni { get; set; }
        public string UserId { get; set; }

        public DateTime? FechaBaja { get; set; }
        public List<string>? roles { get; set; }
        public string? rolCambiar { get; set; }
        public UserStatusAdminViewModel() { }
        public UserStatusAdminViewModel(PersonaViewModel persona) 
        {
            Nombre = persona.Nombre;
            Apellido = persona.Apellido;
            Dni = persona.Dni;
            UserId = persona.IdUsuario;
        }
    }
}
