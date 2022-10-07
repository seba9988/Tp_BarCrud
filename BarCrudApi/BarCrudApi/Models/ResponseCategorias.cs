using BarCrudApi.ViewModels;

namespace BarCrudApi.Models
{
    public class ResponseCategorias
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public IEnumerable<CategoriaViewModel> lista { get; set; }
    }
}
