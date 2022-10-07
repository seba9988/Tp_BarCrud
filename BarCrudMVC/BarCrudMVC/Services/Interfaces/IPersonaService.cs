using BarCrudMVC.Models;

namespace BarCrudMVC.Services.Interfaces
{
	public interface IPersonaService
	{
        //Busca personas con rol Manager sin bar asignado y que no esten con baja
        Task<IList<PersonaViewModel>> GetAllManagersSinBaja();
        //busca con un dni su persona
        Task<PersonaViewModel?> GetOneByDni(string dni);

    }
}
