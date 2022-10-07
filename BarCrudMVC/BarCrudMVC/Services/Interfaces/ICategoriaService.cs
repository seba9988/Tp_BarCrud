using BarCrudMVC.Models;

namespace BarCrudMVC.Services.Interfaces
{
	public interface ICategoriaService
	{
		//Busca categorias sin baja
		Task<List<CategoriaViewModel>?> GetAllSinBaja();
		//busca categorias con baja, solo admins y superAdmins
		Task<List<CategoriaAdminViewModel>?> GetAll();
		//busca una categoria con un id
		Task<CategoriaAdminViewModel?> GetOne(int id);
		//Agrega una categoria
		Task<bool> Add(CategoriaAdminViewModel categoriaVM);
        //Edita una categoria
        Task<bool> Edit(CategoriaAdminViewModel categoriaVM);
		//Baja logica de una categoria 
		Task<bool> SoftDelete(int id);
		//Recuperacion de la baja logica de una categoria
		Task<bool> Restore(int id);
		//Baja permanente de una categoria
		Task<bool> Delete(int id);
	}
}
