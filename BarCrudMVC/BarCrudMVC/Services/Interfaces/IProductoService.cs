using BarCrudMVC.Models;

namespace BarCrudMVC.Services.Interfaces
{
    public interface IProductoService
    {
        //Traigo todos los productos sin baja, para usuario sin loguear o con rol usuario
        Task<List<ProductoViewModel>?> GetAllSinBaja();
        //Traigo productos sin baja de una categoria, para usuario sin loguear o con rol usuario
        Task<List<ProductoViewModel>?> GetAllByCategoria(int id);
        //con un id bar busco productos sin baja de ese bar 
        Task<IList<ProductoViewModel>?> GetAllByBar(int id);
        //busca productos con o sin baja, solo admins y superAdmins
        Task<List<ProductoAdminViewModel>?> GetAll();
        //busca un prodcuto con un id, se trae bar y datos de producto menos fecha baja
        //se usa para usuarios sin loguear, logueados user y manager,
        //si el producto tiene baja logica no se lo trae
        Task<ProductoViewModel?> GetOne(int id);
        //Busca datos de un producto, incluyendo categoria, bar y su fecha baja
        //Se trae el producto tenga o no baja logica
        Task<ProductoAdminViewModel?> GetOneAdmin(int id);
        //Agrega un producto
        Task<bool> Add(ProductoAdminViewModel productoVM);
        //Edita un producto
        Task<bool> Edit(ProductoAdminViewModel productoVM);
        //Baja logica de prodcuto
        Task<bool> SoftDelete(int id);
        //Recuperacion de la baja logica de un prodcuto
        Task<bool> Restore(int id);
        //Baja permanente de un producto
        Task<bool> Delete(int id);
        //Agrega un producto para el bar de un manager
        Task<bool> Add(ProductoManagerViewModel productoVM);
        //Edita un producto del bar de un manager
        Task<bool> Edit(ProductoManagerViewModel productoVM);
        //Baja logica de prodcuto para manager
        Task<bool> SoftDeleteManager(int id);

    }
}
