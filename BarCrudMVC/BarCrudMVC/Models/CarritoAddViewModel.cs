using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
	public class CarritoAddViewModel
	{
		public int ProductoId { get; set; }
        [Range(1, Int16.MaxValue, ErrorMessage = "Ingrese un numero mayor o igual a 1")]
        public short Cantidad { get; set; }
		public string? UserId { get; set; }
	}
}
