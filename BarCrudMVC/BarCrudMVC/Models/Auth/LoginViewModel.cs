using System.ComponentModel.DataAnnotations;

namespace BarCrud.Models.Auth
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
