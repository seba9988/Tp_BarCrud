using BarCrudApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class BarViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una direccion")]
        public string? Direccion { get; set; }
        public string? Imagen { get; set; }

        public BarViewModel() { }
        public BarViewModel(Bar bar)
        {
            Id = bar.Id;
            Nombre = bar.Nombre;
            Direccion = bar.Direccion;
            Imagen = bar.Imagen;
        }
    }
}
