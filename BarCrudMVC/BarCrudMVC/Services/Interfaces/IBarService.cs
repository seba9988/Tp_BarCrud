using BarCrudMVC.Models;

namespace BarCrudMVC.Services.Interfaces
{
    public interface IBarService
    {
        //Busca todos los bares, esten con baja logica o no
        Task<IList<BarAdminViewModel>?> GetAll();
        //Busca todos los bares que no esten de baja, fechaBaja=null
        Task<IList<BarViewModel>> GetAllSinBaja();
        //Busca todos los bares que no esten de baja y no tenga manager, fechaBaja=null
        Task<IList<BarViewModel>> GetAllSinManager();
        //Agrega un bar nueva si este es valido para guardar,false si no se agrego, true si se agrega
        Task<bool> Add(BarAdminViewModel barVM);
        //Edita un bar si esta es valido para editar,false si no se edito, true si se edita
        Task<bool> Edit(BarAdminViewModel barVM);
        //Elimina un bar permanentemente si este es valido para eliminar,false si no se elimino, true si se elimina
        Task<bool> Delete(int id);
        //Elimina un bar logicamente si este es valido para eliminar,false si no se elimino, true si se elimina
        Task<bool> SoftDelete(int id);
        //Recupera un bar eliminado logicamente si estr es valido para recuperar,false si no se recupera, true si se recupera
        Task<bool> Restore(int id);
        //Trae todos los datos de un Bar
        Task<BarViewModel?> GetOne(int id);
        //Busca todos los bares sin baja del manager logueado actualmente
		Task<BarViewModel?> GetAllManager();
        Task<BarAdminViewModel?> GetOneAdmin(int id);
    }
}
