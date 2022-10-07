using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BarCrudApi.Services.Interfaces
{
    public interface IBarService
    {
        //Busca todos los bares, esten con baja logica o no
        Task<IList<BarAdminViewModel>>? GetAll();
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
        //busco los datos basicos de un Bar con un id
        Task<BarViewModel?> GetOne(int id);
        //busco todos los datos de un bar con un id
        Task<BarAdminViewModel> GetOneAdmin(int id);
        //Busca todos los dato del bar perteneciente al id usuario de un manager, solo para managers
        Task<BarViewModel> GetAllManager(string id);
    }
}
