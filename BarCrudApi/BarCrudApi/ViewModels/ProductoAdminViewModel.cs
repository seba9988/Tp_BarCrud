using BarCrudApi.Models;
using System.ComponentModel.DataAnnotations;

namespace BarCrudApi.ViewModels
{
    public class ProductoAdminViewModel
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
        [Required(ErrorMessage = "Se requiere un id bar")]
        public int BarId { get; set; }
       
        public DateTime? FechaBaja { get; set; }
        public BarViewModel? Bar{ get; set; }
        public CategoriaViewModel? Categoria { get; set; }
        public ProductoAdminViewModel() { }
        public ProductoAdminViewModel(Producto producto)
        {
            Id = producto.Id;
            Nombre = producto.Nombre;
            Descripcion = producto.Descripcion;
            Precio = producto.Precio;
            Imagen = producto.Imagen;
            FechaBaja = producto.FechaBaja;
            CategoriaId = producto.Id;
            BarId = producto.BarId;
            Stock = producto.Stock; 
            //Inicializo el bar, si no esta vacio
            if(producto.Bar != null)
                Bar = new BarViewModel(producto.Bar);
            //Inicializo Categoria, si no esta vacia
            if(producto.Categoria != null)
                Categoria = new CategoriaViewModel(producto.Categoria);
        }
    }
}
