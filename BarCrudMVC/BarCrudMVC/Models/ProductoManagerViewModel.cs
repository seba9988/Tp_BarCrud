using System.ComponentModel.DataAnnotations;

namespace BarCrudMVC.Models
{
    public class ProductoManagerViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Se requiere un Nombre")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "Se requiere una Descripcion")]
        public string? Descripcion { get; set; }
        [Required(ErrorMessage = "Se requiere un precio")]
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
        [Required(ErrorMessage = "Se requiere un id categoria")]
        public int CategoriaId { get; set; }
        [Required(ErrorMessage = "Se requiere ingresar un stock")]
        public int Stock { get; set; }

        public string ManagerId { get; set; }
        public int? BarId { get; set; }
        public CategoriaViewModel? Categoria { get; set; }
        public ProductoManagerViewModel() {}
        public ProductoManagerViewModel(ProductoViewModel producto) 
        {
            Id = producto.Id;
            Nombre = producto.Nombre;
            Descripcion = producto.Descripcion;
            Precio = producto.Precio;
            Imagen = producto.Imagen;
            Stock = producto.Stock;
            CategoriaId = producto.CategoriaId;
            BarId = producto.Bar.Id;
            if(producto.Categoria != null)
                Categoria = producto.Categoria;

        }
    }
}
