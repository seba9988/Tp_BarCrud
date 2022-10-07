using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.Services.Interfaces;
using BarCrudApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BarCrudApi.Services
{   
    public class PersonaService:IPersonaService
    {      
        private readonly BarCrudContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public PersonaService(BarCrudContext context, UserManager<IdentityUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<PersonaViewModel?> GetOneByIdUser(string id)
        {
            var user = await _context.Personas.Where(p => p.IdUsuario == id).FirstAsync();
            //relleno el ViewModel con los datos de user
            if (user != null)
            {
                var persona = new PersonaViewModel()
                {
                    Dni = user.Dni,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido
                };
                return (persona);
            }
            return null;
        }
        //Busca datos de la persona con un dni que no este con baja
        public async Task<PersonaViewModel?> GetOneByDni(string dni) 
        {
            var user = await _context.Personas.Where(p => p.Dni == dni ).FirstAsync();
            //relleno el ViewModel con los datos de user
            if (user != null)
            {
                var persona = new PersonaViewModel()
                {
                    Dni = user.Dni,
                    Nombre = user.Nombre,
                    Apellido = user.Apellido
                };
                return (persona);
            }
            return null;
        }
        //busco las personas con rol manager que no tengan baja y no tengan un bar asignado
        public async Task <IList<PersonaViewModel>>GetAllManagersSinBaja()
        {
            //Busco lista de personas que tengan rol manager y no tenga un bar ya asignado
            var managers = await (from per in _context.Personas
                                  join user in _context.Users on per.IdUsuario equals user.Id
                                  join rolUser in _context.UserRoles on user.Id equals rolUser.UserId
                                  join rol in _context.Roles on rolUser.RoleId equals rol.Id
                                  where !(from b in _context.Bares
                                          select b.ManagerDni).Contains(per.Dni)
                                         && rol.Name == UserRoles.Manager
                                         && per.FechaBaja == null
                                  select new PersonaViewModel(per)).ToListAsync();
            return managers;           
        }
    }
}
