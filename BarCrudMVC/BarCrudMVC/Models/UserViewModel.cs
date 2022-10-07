using Microsoft.AspNetCore.Identity;

namespace BarCrudMVC.Models
{
    public class UserViewModel
    {
        public string UserName { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public DateTime? FechaBaja { get; set; }

        public UserViewModel() { }
        public UserViewModel(IdentityUser user)
        {
            UserName = user.UserName;
            Id = user.Id;
            Email = user.Email;
        }

    }
}
