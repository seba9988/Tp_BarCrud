using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class SoftDeleteManagerViewModel
    {
        [Required(ErrorMessage = "Se requiere un ManagerId")]
        public String ManagerId { get; set; }

        [Required(ErrorMessage = "Se requiere un BarId")]
        public int ProductoId { get; set; }
    }
}
