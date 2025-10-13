using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Billetera.Servicio.ServiciosHttp;  

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IHttpServicio, HttpServicio>();  

await builder.Build().RunAsync(); 