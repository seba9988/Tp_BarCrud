using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
	public class CategoriaAdminViewModel
	{
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una Descripcion")]
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public DateTime? FechaBaja { get; set; }    
        public CategoriaAdminViewModel() { }
    }
}
