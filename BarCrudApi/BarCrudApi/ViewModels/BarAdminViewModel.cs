using BarCrudApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class BarAdminViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una direccion")]
        public string? Direccion { get; set; }
        public string? Imagen { get; set; }
        public DateTime? FechaBaja { get; set; }
        public string? ManagerDni { get; set; }
        public PersonaViewModel? manager { get; set; }
        public BarAdminViewModel() { }
        public BarAdminViewModel(Bar bar)
        {
            Id = bar.Id;
            Nombre = bar.Nombre;
            Direccion = bar.Direccion;
            FechaBaja = bar.FechaBaja;
            ManagerDni = bar.ManagerDni;
            Imagen = bar.Imagen;
        }
    }
}
