using BarCrudApi.ViewModels;

namespace BarCrudApi.Models
{
    public class ResponseProductos
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public IEnumerable<ProductoViewModel> lista { get; set; }
    }
}
