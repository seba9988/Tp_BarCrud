using BarCrudApi.Auth;
using BarCrudApi.Models;
using BarCrudApi.ViewModels;
using BarCrudMVC.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace BarCrudApi.Services.Interfaces
{
    public class UserManagementService: IUserManagementService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IPersonaService _personaService;
        private readonly BarCrudContext _context;

        public UserManagementService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            BarCrudContext context,
            IPersonaService personaService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _context = context;
            _personaService = personaService;
        }
        //Traigo todos los usuarios 
        public async Task<IList<UserViewModel>>? GetAll()
        {
            var users = await (from user in _context.Users
                               join p in _context.Personas on user.Id equals p.IdUsuario
                               select new UserViewModel(user)
                               {
                                   FechaBaja = p.FechaBaja

                               }).ToListAsync();
            return users;
            //var users =  _context.Users.Select(async u =>   new UserViewModel
            //{
            //    UserName = u.UserName,
            //    Id = u.Id,
            //    Email = u.Email,
            //    Roles = await _userManager.GetRolesAsync(u)
            //}).ToList();
            //return users
        }
        //Traigo todos los datos de un usuario
        public async Task<UserStatusAdminViewModel>? GetOne(string id)
        {
            var userData = new UserStatusAdminViewModel(await _personaService.GetOneByIdUser(id));
            var user = _context.Users.Where(u => u.Id == id).FirstOrDefault();
            if(user != null) 
            {
                //Agrego datos de user
                userData.UserName = user.UserName;
                userData.Email = user.Email;
                userData.UserId = user.Id;
                //busco roles
                var roles = await _userManager.GetRolesAsync(user);
                if(roles != null) 
                {
                    userData.roles = new List<string>();
                    foreach(var role in roles) 
                    {

                        userData.roles.Add(role);
                    }
                }
            }            
            return userData;
        }
        //Se edita al usuario, puede ser roles u otros atributos
        public async Task<bool> Edit(UserStatusAdminViewModel user) 
        {
            //busco la persona y usuario actuales con userId y dni
            var usuarioActual = _context.Users.Find(user.UserId);
            var personaActual = _context.Personas.Find(user.Dni);
            //si la persona y el usuario existen los actualizo
            if (usuarioActual != null && personaActual != null)
                try
                {
                    //actualizo los datos de usuario actual
                    usuarioActual.UserName = user.UserName;
                    usuarioActual.Email = user.Email;
                    //Actualizo los datos de persona actual
                    personaActual.Nombre = user.Nombre;
                    personaActual.Apellido= user.Apellido;
                    //Actualizo sus roles dependiendo del valor de rolCambiar
                    //Si falla el metodo de cambiar rol devuelvo falso y no guardo los cambios al usuario
                    switch (user.rolCambiar) 
                    {
                        case UserRoles.User:
                            if(! await CambiarRolUser(usuarioActual))
                                return false;
                            break;
                        case UserRoles.Manager:
                            if (!await CambiarRolManager(usuarioActual))
                                return false;
                            break;
                        case UserRoles.Admin:
                            if (!await CambiarRolAdmin(usuarioActual))
                                return false;
                            break;
                        case UserRoles.SuperAdmin:
                            if (!await CambiarRolSuperAdmin(usuarioActual))
                                return false;
                            break;
                        default: return false;
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            return false;
        }
        //Baja logica de persona/usuario, este no puede loguearse
        public async Task<bool> SoftDelete(string id) 
        {
            var persona = await _context.Personas.Where(p => p.IdUsuario == id).FirstAsync();

            //Si existe la persona asociada al usuario le doy baja
            if (persona != null)
                try
                {
                    persona.FechaBaja = DateTime.UtcNow;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            //aca podria tirar una custom Exception para informar que el id no existe
            return false;
        }
        //Recuperacion de la baja logica
        public async Task<bool> Restore(string id)
        {
            var persona = await _context.Personas.Where(p => p.IdUsuario == id).FirstAsync();

            //Si existe la persona asociada al usuario la recupero
            if (persona != null)
                try
                {
                    persona.FechaBaja = null;

                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            //aca podria tirar una custom Exception para informar que el id no existe
            return false;
        }
        //Baja permanente del usuario/persona, sus pedidos y detalles pedidos
        public async Task<bool> Delete(string id) 
        {
            var userEliminar = await _context.Users.FindAsync(id);
            var personaEliminar = await _context.Personas.Where(p => p.IdUsuario == id).FirstAsync();
            //Definimos la logica de eliminacion, si existe se borra
            if (userEliminar != null && personaEliminar != null)
            {
                try
                {
                    _context.Users.Remove(userEliminar);
                    _context.Personas.Remove(personaEliminar);
                    await _context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        //Logueo al usuario, y se verifica si esta dado de baja la persona correspondiente al usuario
        public async Task<ResponseLogin> Login(LoginViewModel model) 
        {
            //Instancio ResponseLogin en null
            var reply = new ResponseLogin
            {
                Token = null,
                Expiration = null
            };
            var user = await _userManager.FindByNameAsync(model.UserName);
            //Busco la persona correspondiente al usuario
            var personaConBaja = await _context.Personas.Where(p => p.IdUsuario == user.Id).FirstAsync();
            //Si la password es correcta y la persona no tiene baja genero token

            if (await _userManager.CheckPasswordAsync(user, model.Password) && personaConBaja.FechaBaja == null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserId",user.Id)
                };
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }
                //creo el token con las claims 
                var token = GetToken(authClaims);

                reply.Token = new JwtSecurityTokenHandler().WriteToken(token);
                reply.Expiration = token.ValidTo;
            }
            return reply;
        }

        public async Task<bool> Register(RegisterViewModel model) 
        {
            try 
            {
                //valido que el dni y nombre usuario no exista, y si todo esta correcto creo el IdentityUser
                var user = await ValidarYCrearUser(model);

                if (user == null)
                    return false;

                var result = await _userManager.CreateAsync(user, model.Password);
                //Verifico que se cree con exito el usuario con su password
                if (!result.Succeeded)
                    return false;
                //si no existe el rol en la base de dato lo creo
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                //agrego rol al usuario
                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                //Agrego el usuario a la tabla de clientes
                if(await AddPersona(model, user)) 
                {
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }
        public async Task<bool> RegisterManager(RegisterManagerViewModel model) 
        {
            try 
            {
                //valido que el dni y nombre usuario no exista, y si todo esta correcto creo el IdentityUser
                var user =  await ValidarYCrearManager(model);
                
                if (user == null)
                    return false;

                //Se guarda en base de datos el usuario si paso las verificaciones
                var result = await _userManager.CreateAsync(user, model.Password);

                //Verifico que se cree con exito el usuario con su password
                if (!result.Succeeded)
                    return false;

                // verifico que los roles user y manager existan, sino los creo
                if (!await _roleManager.RoleExistsAsync(UserRoles.Manager))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                //agrego roles al manager de user y manager 
                if (await _roleManager.RoleExistsAsync(UserRoles.Manager))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Manager);
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }

                //se agrega persona a la base de datos
                await AddManager(model, user);
                //true si se realiza todo con exito
                return true;
            }
            catch (Exception) { return false; }           
        }
        public async Task<bool> RegisterAdmin(RegisterViewModel model) 
        {
            try
            {
                //valido que el dni y nombre usuario no exista, y si todo esta correcto creo el IdentityUser
                var user = await ValidarYCrearUser(model);

                if (user == null)
                    return false;

                var result = await _userManager.CreateAsync(user, model.Password);
                // verifico que los roles user, manager y admin existan, sino los creo
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Manager))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                //agrego roles al admin de user y admin
                if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                //se agrega persona a la base de datos
                await AddPersona(model, user);
                //true si se realiza todo con exito
                return true;
            }
            catch (Exception) { return false; }
        }
        public async Task<bool> RegisterSuperAdmin(RegisterViewModel model) 
        {
            try
            {
                //valido que el dni y nombre usuario no exista, y si todo esta correcto creo el IdentityUser
                var user = await ValidarYCrearUser(model);

                if (user == null)
                    return false;

                var result = await _userManager.CreateAsync(user, model.Password);
                // verifico que los roles user, manager, admin y superAdmin existan, sino los creo
                if (!await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.SuperAdmin));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
                if (!await _roleManager.RoleExistsAsync(UserRoles.Manager))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.Manager));
                if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                    await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));
                //agrego roles al SuperAdmin  de user, admin y SuperAdmin
                if (await _roleManager.RoleExistsAsync(UserRoles.SuperAdmin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.SuperAdmin);
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
                if (await _roleManager.RoleExistsAsync(UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }

                //se agrega persona a la base de datos
                await AddPersona(model, user);
                //true si se realiza todo con exito
                return true;
            }
            catch (Exception) { return false; }
        }
        //Cambio rol del usuario con el userName de usuario, los demas roles se sacan
        public async Task<bool> CambiarRolUser(IdentityUser user)
        {
            try
            {
                //Verifico que no venga vacio el user
                if (user == null)
                    return false;
                //remuevo roles que no sean usuario si los tiene
                if (await _userManager.IsInRoleAsync(user,UserRoles.SuperAdmin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.SuperAdmin);
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
                }
                //Si es manager busco sus bares y se los saco
                if (await _userManager.IsInRoleAsync(user, UserRoles.Manager))
                {
                    //Saco rol de manager y si tiene bares a cargo cambio managerDni de bar a null
                    await RemoveRolManager(user);
                }
                //si no tiene rol User se lo agrego
                if (!await _userManager.IsInRoleAsync(user, UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                return true;
            }
            catch (Exception) { return false; }
        }
        //Cambio rol del usuario con el userName de usuario, los demas roles se sacan
        public async Task<bool> CambiarRolManager(IdentityUser user)
        {
            try
            {
                //Verifico que no venga vacio el user
                if (user == null)
                    return false;
                //remuevo roles que no sean manager y user si los tiene
                if (await _userManager.IsInRoleAsync(user, UserRoles.SuperAdmin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.SuperAdmin);
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.Admin);
                }
                //si no tiene rol SuperAdmin, admin o user se los agrego
                if (!await _userManager.IsInRoleAsync(user, UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                if (!await _userManager.IsInRoleAsync(user, UserRoles.Manager))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Manager);
                }
                return true;
            }

            catch (Exception) { return false; }
        }
        //Cambio rol del usuario con el userName de usuario, los demas roles se sacan
        public async Task<bool> CambiarRolAdmin(IdentityUser user)
        {
            try
            {
                //Verifico que no venga vacio el user
                if (user == null)
                    return false;
                //remuevo roles que no sean Admin y user si los tiene
                if (await _userManager.IsInRoleAsync(user, UserRoles.SuperAdmin))
                {
                    await _userManager.RemoveFromRoleAsync(user, UserRoles.SuperAdmin);
                }
                if (await _userManager.IsInRoleAsync(user, UserRoles.Manager))
                {
                    //Saco rol de manager y si tiene bares a cargo cambio managerDni de bar a null
                    await RemoveRolManager(user);
                }
                //si no tiene rol User o admin se los agrego
                if (!await _userManager.IsInRoleAsync(user, UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                if (!await _userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
                return true;
            }
            catch (Exception) { return false; }
        }

        //Cambio rol del usuario con el userName de usuario, los demas roles se sacan
        public async Task<bool> CambiarRolSuperAdmin(IdentityUser user)
        {
            try
            {
                //Verifico que no venga vacio el user
                if (user == null)
                    return false;
                //remuevo roles que no sean SuperAdmin,Admin y user si los tiene
                if (await _userManager.IsInRoleAsync(user, UserRoles.Manager))
                {
                    //Saco rol de manager y si tiene bares a cargo cambio managerDni de bar a null
                    await RemoveRolManager(user);
                }
                //si no tiene rol SuperAdmin, admin o user se los agrego
                if (!await _userManager.IsInRoleAsync(user, UserRoles.User))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.User);
                }
                if (!await _userManager.IsInRoleAsync(user, UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
                if (!await _userManager.IsInRoleAsync(user, UserRoles.SuperAdmin))
                {
                    await _userManager.AddToRoleAsync(user, UserRoles.SuperAdmin);
                }

                return true;
            }
            catch (Exception) { return false; }
        }
        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,

                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }
        //Se crea la persona y asocia idUduario 
        private async Task<bool> AddPersona(RegisterViewModel model, IdentityUser userCreado)
        {
            var persona = new Persona
            {
                Dni = model.Dni,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                IdUsuario = userCreado.Id
            };
            //Agrego persona la base de datos
            await _context.Personas.AddAsync(persona);
            await _context.SaveChangesAsync();
            return true;
        }
        //Se crea la persona, asocia idUduario y dni al bar si es que se selecciono uno
        private async Task<bool> AddManager(RegisterManagerViewModel model, IdentityUser userCreado)
        {
            var persona = new Persona
            {
                Dni = model.Dni,
                Nombre = model.Nombre,
                Apellido = model.Apellido,
                IdUsuario = userCreado.Id
            };
            //comienzo transaccion
            var transaction = _context.Database.BeginTransaction();

            //Traigo el bar y le asigno el dni del manager
            var bar = _context.Bares.Find(model.BarId);
            if(bar != null)
                bar.ManagerDni = model.Dni;

            //Agrego persona la base de datos
            await _context.Personas.AddAsync(persona);
            //Guardo cambios
            await _context.SaveChangesAsync();

            //Si todo sale bien se hace commit, sino automaticamente se hace rollBack segun la documentacion
            await transaction.CommitAsync();
            
            return true;
        }
        //Se valida que el userName no este usado, y que el dni no tenga ya asignada una cuenta
        //solo se permite una cuenta por dni
        //si todo esta correcto se crea y devuelve el IdentityUser
        private async Task<IdentityUser?> ValidarYCrearUser(RegisterViewModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            //verifico que el usuario no exista
            if (userExist != null)
                return null;
            var personaExist = await _context.Personas.FindAsync(model.Dni);
            //verifico que el la persona del dni ingresado no tenga ya un usuario
            
            if (personaExist != null)
                return null;
            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            return user;
        }
        private async Task<IdentityUser?> ValidarYCrearManager(RegisterManagerViewModel model)
        {
            var userExist = await _userManager.FindByNameAsync(model.UserName);
            //verifico que el usuario no exista
            if (userExist != null)
                return null;
            //Verifico que no exista una persona ya con ese dni
            var personaExist = await _context.Personas.FindAsync(model.Dni);
            //verifico que el la persona del dni ingresado no tenga ya un usuario

            if (personaExist != null)
                return null;
            //verifico que si el var ingresado existe no tenga ya un manager
            var bar = await _context.Bares.FindAsync(model.BarId);
            if (bar != null)
            {
                //Verifico que el bar no tenga ya un manager
                if (bar.ManagerDni != null)
                    return null;
            }           
            IdentityUser user = new()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.UserName
            };
            return user;
        }
        //Remuevo rol manager y si tiene bares a cargo saco su dni de dichos bares
        private async Task RemoveRolManager(IdentityUser user) 
        {
            //busco la persona
            var persona = await _context.Personas.Where(p => p.IdUsuario == user.Id).FirstAsync();
            //Busco bares del manager
            var bares = await _context.Bares.Where(b => b.ManagerDni == persona.Dni).ToListAsync();
            //Saco el dni manager de sus bares
            foreach (var bar in bares)
            {
                bar.ManagerDni = null;
            }
            await _context.SaveChangesAsync();
            //Saco rol manager
            await _userManager.RemoveFromRoleAsync(user, UserRoles.Manager);
        }
    }

}