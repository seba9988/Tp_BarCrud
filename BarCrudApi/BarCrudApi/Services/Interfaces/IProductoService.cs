using BarCrudApi.ViewModels;

namespace BarCrudApi.Services.Interfaces
{
    public interface IProductoService
    {
        //Busca todos los productos, esten con baja logica o no
        Task<IList<ProductoAdminViewModel>> GetAll();
        //Traigo un producto con su bar , solo si no esta con baja logica
        Task<ProductoViewModel?> GetOne(int id);
        //Busco un producto completo para admin
        Task<ProductoAdminViewModel?> GetOneAdmin(int id);
        //Busca todos los productos que no esten de baja, fechaBaja=null
        Task<IList<ProductoViewModel>> GetAllSinBaja();
        //Busca productos sin baja de una Categoria
        Task<IList<ProductoViewModel>> GetAllByCategoria(int id);
        //Busca productos sin baja de un bar
        Task<IList<ProductoViewModel>> GetAllByBar(int id);
        //Agrega un producto nuevo si esta es valida para guardar,false si no se agrego, true si se agrega
        Task<bool> Add(ProductoAdminViewModel categoriaVM);
        //Edita un producto si este es valido para editar,false si no se edito, true si se edita
        Task<bool> Edit(ProductoAdminViewModel categoriaVM);

        //Elimina un producto permanentemente si este es valido para eliminar,false si no se elimino, true si se elimina
        Task<bool> Delete(int id);

        //Elimina un producto logicamente si este es valido para eliminar,false si no se elimino, true si se elimina
        Task<bool> SoftDelete(int id);

        //Recupera un prodcuto eliminado logicamente si este es valida para recuperar,false si no se recupera, true si se recupera
        Task<bool> Restore(int id);

        //Agrega un producto al bar del manager si el manager/bar no tienen baja
        Task<bool> AddManager(ProductoManagerViewModel productoVM);

        //Edita un producto si este pertenece al bar del manager
        Task<bool> EditManager(ProductoManagerViewModel categoriaVM);

        //Elimina un producto logicamente si este pertenece al bar del manager
        Task<bool> SoftDeleteManager(SoftDeleteManagerViewModel categoriaVM);   
    }
}
