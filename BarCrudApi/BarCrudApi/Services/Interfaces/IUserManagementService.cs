using BarCrudApi.Auth;
using BarCrudApi.ViewModels;
using BarCrudMVC.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BarCrudApi.Services.Interfaces
{
    public interface IUserManagementService
    {
        //Login de usuario, devuelve un jwt y su fecha de caducidad
        Task<ResponseLogin> Login(LoginViewModel model);
        //Registra un usuario con rol usuario
        Task<bool> Register(RegisterViewModel model);
        //Registra un usuario con rol Manager
        Task<bool> RegisterManager(RegisterManagerViewModel model);
        //Registra un usuario con rol admin
        Task<bool> RegisterAdmin(RegisterViewModel model);
        //Registra un usuario con rol superAdmin
        Task<bool> RegisterSuperAdmin(RegisterViewModel model);
        //Cambia el rol de un usuario a usuario
        Task<bool> CambiarRolUser(IdentityUser user);
        //Cambia el rol de un usuario a manager
        Task<bool> CambiarRolManager(IdentityUser user);
        //Cambia el rol de un usuario a admin
        Task<bool> CambiarRolAdmin(IdentityUser user);
        //Cambia el rol de un usuario a superAdmin
        Task<bool> CambiarRolSuperAdmin(IdentityUser user);
        //Cambia el rol de un usuario a superAdmin
        Task<IList<UserViewModel>>? GetAll( );
        //Busco los datos de un usuario, persona y sus roles en base a su id
        Task<UserStatusAdminViewModel> GetOne(string id);
        //Se edita un usuario, puede ser roles u atributos
        Task<bool> Edit(UserStatusAdminViewModel user);
        //Baja logica de persona/usuario, este no puede loguearse
        Task<bool> SoftDelete(string id);
        //Baja permanente del usuario/persona
        Task<bool> Delete(string id);
        //Recuperacion de la baja logica
        Task<bool> Restore(string id);


    }
}
