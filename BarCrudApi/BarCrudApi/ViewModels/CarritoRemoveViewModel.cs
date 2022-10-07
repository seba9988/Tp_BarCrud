using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class CarritoRemoveViewModel
    {
        [Required(ErrorMessage = "ProductoId is required")]
        public int ProductoId { get; set; }
        [Required(ErrorMessage = "User Id is required")]
        public string? UserId { get; set; }
    }
}
