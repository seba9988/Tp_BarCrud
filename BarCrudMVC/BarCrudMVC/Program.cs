using BarCrud.Models.Auth;
using BarCrudMVC.Services;
using BarCrudMVC.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//Autenticacion
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        //Path o direccion del login
        options.LoginPath = "/UserManagement/Index";
        options.AccessDeniedPath = "/Home/AccessDenied";

    });
//Agrego razor, sesiones 
builder.Services.AddRazorPages();
builder.Services.AddSession();
// se agrega service controladores
builder.Services.AddControllers();
//Agrego HttpContextAccesor
builder.Services.AddHttpContextAccessor();
//Agrego HttpClient de la api para hacer inyeccion de dependencia
builder.Services.AddHttpClient("BarApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7074");
});
//Agrego servicios, scope y singleton
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IBarService, BarService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCookiePolicy();

app.UseSession();
app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
