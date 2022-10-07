using BarCrudApi.ViewModels;

namespace BarCrudApi.Services.Interfaces
{
    public interface IPersonaService
    {
        //Busca datos de la persona con un UserId sin incluir fechaBaja
        Task<PersonaViewModel?> GetOneByIdUser(string id);
        //Busca datos de la persona con un dni sin incluir fechaBaja
        Task<PersonaViewModel?> GetOneByDni(string dni);
        //Busca personas con rol Manager  sin bar asignado y que no esten con baja
        Task <IList<PersonaViewModel>> GetAllManagersSinBaja();
    }
}
