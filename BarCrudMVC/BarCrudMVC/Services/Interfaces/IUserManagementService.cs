using BarCrud.Models.Auth;
using BarCrudMVC.Models;
using BarCrudMVC.Models.Auth;

namespace BarCrudMVC.Services.Interfaces
{
	public interface IUserManagementService
	{
		//Se realiza el login de un usuario
		Task<bool> Login (LoginViewModel loginModel);
		//Registro de un usuario con role user
		Task<bool> Register(RegisterViewModel registerModel);
        //Se registra un usuario con rol admin, solo superAdmins
        Task<bool> RegisterManager(RegisterManagerViewModel registerModel);
        //Se registra un usuario con rol admin, solo superAdmins
        Task<bool> RegisterAdmin(RegisterViewModel registerModel);
        //Se registra un usuario con rol admin, solo superAdmins
        Task<bool> RegisterSuperAdmin(RegisterViewModel registerModel);
        //LogOut de usuario y se borra jwt de la coockie
        Task LogOut();
        //se busca y muestra datos de la persona asociada el UserId y datos del usuario
        Task<UserStatusViewModel?> UserStatus();
        //busco todos los usuarios con o sin baja
        Task<List<UserViewModel>?> GetAll();
        //busco datos del usuario
        Task<UserStatusAdminViewModel?> GetOne(string id);
        //Se edita al usuario, puede ser roles u otros atributos
		Task<bool> Edit(UserStatusAdminViewModel user);
        //Baja logica de persona/usuario, este no puede loguearse
        Task<bool> SoftDelete(string id);
        //Baja permanente del usuario/persona
        Task<bool> Delete(string id);
        //Recuperacion de la baja logica
        Task<bool> Restore(string id);
    }
}
