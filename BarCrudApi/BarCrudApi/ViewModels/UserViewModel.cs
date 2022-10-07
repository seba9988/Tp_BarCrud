using BarCrudApi.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class UserViewModel
    {
        public string UserName { get; set; }

        public string Id { get; set; }

        public string Email { get; set; }

        public DateTime? FechaBaja { get; set; }
        public IList<string> Roles { get; set; }

        public UserViewModel() { }
        public UserViewModel(IdentityUser user)
        {
            UserName = user.UserName;
            Id = user.Id;
            Email = user.Email;
        }

    }
}
