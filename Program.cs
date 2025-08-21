using Inmobiliaria.Data;
using Inmobiliaria.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
// DI
builder.Services.AddScoped<IMySqlConnectionFactory, MySqlConnectionFactory>();
builder.Services.AddScoped<IPersonaRepository, PersonaRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();   
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Personas}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
