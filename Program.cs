using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 🔹 Probar conexión acá
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
try
{
    using var conn = new MySqlConnection(connStr);
    conn.Open();

    var sql = "SELECT id, DNI, nombre, apellido, contacto FROM personas";
    using var cmd = new MySqlCommand(sql, conn);
    using var reader = cmd.ExecuteReader();

    Console.WriteLine("===== Datos de Personas =====");
    while (reader.Read())
    {
        Console.WriteLine(
            $"ID: {reader.GetInt32("id")}, " +
            $"DNI: {reader.GetString("DNI")}, " +
            $"Nombre: {reader.GetString("nombre")}, " +
            $"Apellido: {reader.GetString("apellido")}, " +
            $"Contacto: {reader.GetString("contacto")}"
        );
    }
    Console.WriteLine("=============================");
}
catch (Exception ex)
{
    Console.WriteLine("❌ Error de conexión o consulta: " + ex.Message);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
