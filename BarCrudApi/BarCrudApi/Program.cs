using BarCrudApi.Models;
using BarCrudApi.Services;
using BarCrudApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
// Add services to the container.
//Conexion a SQLSERVER
builder.Services.AddSqlServer<BarCrudContext>(builder.Configuration.GetConnectionString("MiDB"));

builder.Services.AddDbContext<BarCrudContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cnTareas")));
// agrego Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(x =>
{
    x.Password.RequiredLength = 6;
    x.Password.RequireNonAlphanumeric = false;
}).
    AddEntityFrameworkStores<BarCrudContext>()
    .AddDefaultTokenProviders();
//Autenticacion
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})

//Agregamos Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = configuration["JWT:ValidAudience"],
        ValidIssuer = configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
    };
});
// se agrega service controladores
builder.Services.AddControllers();
//Agrego Servicios Scope y singleton
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddScoped<IBarService, BarService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Autenticacion y autorizacion
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
