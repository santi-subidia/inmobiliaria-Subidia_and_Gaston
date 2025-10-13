using Inmobiliaria.Data;
using Inmobiliaria.Repositories;
using Inmobiliaria.Services;                  // ⟵ AuthService (agregar namespace)
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // ⟵ Cookie auth

var builder = WebApplication.CreateBuilder(args);

// 🔗 Cadena de conexión (definida en appsettings.json -> "DefaultConnection")
var cs = builder.Configuration.GetConnectionString("DefaultConnection");

// ==============================
// Dependencias (Repos / Factory)
// ==============================
builder.Services.AddScoped<IMySqlConnectionFactory, MySqlConnectionFactory>();

builder.Services.AddScoped<IInmuebleRepository, InmuebleRepository>();
builder.Services.AddScoped<ITipoInmuebleRepository, TipoInmuebleRepository>();
builder.Services.AddScoped<IInquilinoRepository, InquilinoRepository>();
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepository>();
builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IImagenRepository, ImagenRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();


// ==============================
// Autenticación por Cookies
// ==============================

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath        = "/Auth/Login";
        options.LogoutPath       = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name      = "Inmobiliaria.Auth";
        options.Cookie.HttpOnly  = true;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan    = TimeSpan.FromHours(8);
    });

// Configurar políticas de autorización
builder.Services.AddAuthorization(options =>
{
    // Política para administradores solamente
    options.AddPolicy("Administrador", policy => 
        policy.RequireRole("Administrador"));
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Configurar archivos estáticos para la carpeta Uploads
var uploadsPath = Path.Combine(app.Environment.WebRootPath, "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/Uploads"
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
