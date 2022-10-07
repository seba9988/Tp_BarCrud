using BarCrudApi.ViewModels;

namespace BarCrudApi.Services.Interfaces
{
    public interface ICategoriaService
    {
        //Busca todas las categorias, esten con baja logica o no
        Task<IList<CategoriaAdminViewModel>> GetAll();
        //Ubsca todas las categorias con sus productos, esten con baja logica o no
        Task<IList<CategoriaProdAdminViewModel>> GetAllCategoriasProductos();
        //Busca todas las categorias que no esten de baja, fechaBaja=null
        Task<IList<CategoriaViewModel>> GetAllSinBaja();
        //Agrega una categoria nueva si esta es valida para guardar,false si no se agrego, true si se agrega
        Task<bool> Add(CategoriaViewModel categoriaVM);
        //Edita una categoria si esta es valida para editar,false si no se edito, true si se edita
        Task<bool> Edit(CategoriaViewModel categoriaVM);
        //Elimina una categoria permanentemente si esta es valida para eliminar,false si no se elimino, true si se elimina
        Task<bool> Delete(int id);
        //Elimina una categoria logicamente si esta es valida para eliminar,false si no se elimino, true si se elimina
        Task<bool> SoftDelete(int id);
        //Recupera una categoria eliminada logicamente si esta es valida para recuperar,false si no se recupera, true si se recupera
        Task<bool> Restore(int id);
        Task<CategoriaAdminViewModel?> GetOne(CategoriaAdminViewModel categoriaVM);
    }
}
