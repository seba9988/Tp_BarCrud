using BarCrud.Models.Auth;
using BarCrudMVC.Models;
using BarCrudMVC.Models.Auth;
using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BarCrudMVC.Services
{
	public class UserManagementService: BaseService, IUserManagementService
	{
        public UserManagementService(
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor):
            base(httpContextAccessor, httpClientFactory)
        { }
        //busco todos los usuarios con o sin baja
        public async Task<List<UserViewModel>?> GetAll()
        {
            try
            {
                //Busco las categorias con y sin bajas
                var response = await client.GetAsync("/api/Authenticate");
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<UserViewModel>>(result);
                    return users;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //busca un usuario con su persona y roles
        public async Task<UserStatusAdminViewModel?> GetOne(string id)
        {
            try
            {
                //Busco los correspondientes al usuario
                var response = await client.GetAsync("/api/Authenticate/user/" + id);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UserStatusAdminViewModel>(result);
                    return user;
                }
                return null;
            }
            catch (Exception) { return null; }
        }
        //Se edita al usuario puede ser roles u atributos
        public async Task<bool> Edit(UserStatusAdminViewModel user) 
        {
            try
            {
                var id = user.UserId;
                //envio el producto a la api para ser editado
                var response = await client.PutAsJsonAsync($"/api/Authenticate/{id}", user);
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        public async Task<bool> SoftDelete(string id) 
        {
            try
            {
                //Realizo la baja logica en la api
                var response = await client.PostAsJsonAsync($"/api/Authenticate/softDelete/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Recuperacion de la baja logica
        public async Task<bool> Restore(string id)
        {
            try
            {
                //Realizo recuperacion logica en la api
                var response = await client.PostAsJsonAsync($"/api/Authenticate/restore/{id}", "");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Baja permanente del usuario/persona
        public async Task<bool> Delete(string id) 
        {
            try
            {
                //Realizo la baja permanente en la api
                var response = await client.DeleteAsync($"/api/Authenticate/{id}");
                response.EnsureSuccessStatusCode();
                return response.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }

        //Se realiza el login de un usuario
        public async Task<bool> Login(LoginViewModel loginModel) 
        {
            try 
            {
                //HTTP POST a la api para hacer el login
                var postTask = await client.PostAsJsonAsync<LoginViewModel>("api/Authenticate/login", loginModel);

                if (postTask.IsSuccessStatusCode)
                {
                    //leo el resultado de la llamada a la api
                    var response = postTask.Content.ReadAsStringAsync().Result;
                    //Desserializo el json
                    var resp = JsonConvert.DeserializeObject<GetLoginResponse>(response);
                    //recupero el token y los claims
                    var token = resp.Token;
                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(token);
                    
                    var userNameToken = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type.Contains("name") );
                    var roleToken = jwtSecurityToken.Claims.Where(c => c.Type.Contains("role"));
                    var emailToken = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress"));
                    var idUserToken = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "UserId");
                    //Agrego claims del token a una lista
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,userNameToken.Value),
                        new Claim(ClaimTypes.Email, emailToken.Value),
                        new Claim("UserId",idUserToken.Value)
                    };
                    var claimsIdentity = new ClaimsIdentity(authClaims, CookieAuthenticationDefaults.AuthenticationScheme);
                    //agrego role claims
                    foreach (var role in roleToken)
                    {
                        claimsIdentity.AddClaim(role);
                    }
                    var principal = new ClaimsPrincipal(claimsIdentity);

                    //Realizo el logeo con coockie
                    await _contextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    //Guardo token en una cookie de tipo HttpOnly
                    _contextAccessor.HttpContext.Response.Cookies.Append(XAccessToken, resp.Token,
                        new CookieOptions { HttpOnly = true, Secure = true });
                    //return View(resp);
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
         
        }
        //Registro de un usuario con role user
        public async Task<bool> Register(RegisterViewModel registerModel) 
        {
            try
            {
                //HTTP POST para realizar el registro
                var postTask = await client.PostAsJsonAsync<RegisterViewModel>("/api/Authenticate/register", registerModel);

                var response = await postTask.Content.ReadAsStringAsync();

                return postTask.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) {return false; }          
        }
        //Registro de un usuario con role user
        public async Task<bool> RegisterManager(RegisterManagerViewModel registerModel)
        {
            try
            {
                //HTTP POST para realizar el registro
                var postTask = await client.PostAsJsonAsync<RegisterManagerViewModel>("/api/Authenticate/register/manager", registerModel);

                var response = await postTask.Content.ReadAsStringAsync();

                return postTask.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Registro un usuario con rol Admin
        public async Task<bool> RegisterAdmin(RegisterViewModel registerModel) 
        {
            try
            {
                //HTTP POST para realizar el registro
                var postTask = await client.PostAsJsonAsync<RegisterViewModel>("/api/Authenticate/register/admin", registerModel);

                var response = await postTask.Content.ReadAsStringAsync();

                return postTask.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //Registro un usuario con rol superAdmin
        public async Task<bool> RegisterSuperAdmin(RegisterViewModel registerModel)
        {
            try
            {
                //HTTP POST para realizar el registro
                var postTask = await client.PostAsJsonAsync<RegisterViewModel>("/api/Authenticate/register/SuperAdmin", registerModel);

                var response = await postTask.Content.ReadAsStringAsync();

                return postTask.IsSuccessStatusCode ? true : false;
            }
            catch (Exception) { return false; }
        }
        //LogOut de usuario y se borra jwt de la coockie
        public async Task LogOut() 
        {
            await _contextAccessor.HttpContext.SignOutAsync();
            //Elimino la coockie poniendola como espacio
            _contextAccessor.HttpContext.Response.Cookies.Append(XAccessToken, "",
                        new CookieOptions { HttpOnly = true, Secure = true });

        }
        //se busca y muestra datos de la persona asociada el UserId y datos del usuario
        public async Task<UserStatusViewModel?> UserStatus() 
        {
            var userId = _contextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type.Contains("UserId"))?.Value;

            if (userId != null)
            {
                //HTTP get
                HttpResponseMessage response = await client.GetAsync("/api/Persona/" + userId);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var personaData = JsonConvert.DeserializeObject<PersonaViewModel>(result);

                    //busco roles en el token
                    var roleToken = _contextAccessor.HttpContext.User.Claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                    //paso datos del usuario contenidos en el token y personaData al viewModel
                    var userStatus = new UserStatusViewModel
                    {
                        UserId = userId,
                        Dni = personaData.Dni,
                        Nombre = personaData.Nombre,
                        Apellido = personaData.Apellido,
                        Email = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("emailaddress"))?.Value,
                        UserName = _contextAccessor.HttpContext.User.Claims.First(c => c.Type.Contains("name"))?.Value,
                        roles = roleToken.ToList()
                };
                        
                    return userStatus;
                }                
            }
            //si no se encuentra el id en el token quiere decir que tiene algun error
            //se desloguea el usuario
            return null;
        }
    }
}
