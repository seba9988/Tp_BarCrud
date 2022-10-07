using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.Auth
{
    public class RegisterManagerViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Required(ErrorMessage = "User Name is required"), EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Nombre is required")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Apellido is required")]
        public string Apellido { get; set; }
        [Required(ErrorMessage = "Dni is required"), MinLengthAttribute(8), MaxLength(8)]
        [Range(0, Int64.MaxValue, ErrorMessage = "Dni should not contain characters")]
        public string Dni { get; set; }
        public int? BarId { get; set; }
    }
}
