using BarCrudApi.ViewModels;

namespace BarCrudApi.Models
{
    public class ResponseAdminCategorias
    {
        public string? Status { get; set; }
        public string? Message { get; set; }
        public IEnumerable<CategoriaAdminViewModel> lista { get; set; }
    }
}
