using Billetera.BD.Datos;
using Billetera.BD.Datos.Entity;
using Billetera.Repositorio.Repositorio;
using Billetera.Server.Client.Pages;
using Billetera.Server.Components;
using BilleteraVirtual.Repositorio.Repositorios;
using Microsoft.EntityFrameworkCore;
using Billetera.Servicio.ServiciosHttp;


//configura el constructor de la aplicacion
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("ConnSqlServer")
                            ?? throw new InvalidOperationException(
                                    "El string de conexion no existe.");
builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<IUsuariosRepositorio<Usuarios>, UsuariosRepositorio>();
builder.Services.AddScoped<IBilleteraRepositorio, BilleteraRepositorio>();
builder.Services.AddScoped<IMonedaRepositorio, MonedaRepositorio>();
builder.Services.AddScoped<IRepositorio<Moneda>, Repositorio<Moneda>>();
builder.Services.AddScoped<IRepositorio<TipoCuenta>, Repositorio<TipoCuenta>>();
builder.Services.AddScoped<IRepositorio<Cuenta>, Repositorio<Cuenta>>();
builder.Services.AddScoped<ICuentaRepositorio, CuentaRepositorio>();

///front
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5223") });
builder.Services.AddScoped<IHttpServicio, HttpServicio>();
// construccion de la aplicacion
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Billetera.Server.Client._Imports).Assembly);

app.MapControllers();
app.Run();
