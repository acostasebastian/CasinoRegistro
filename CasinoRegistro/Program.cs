using CasinoRegistro.Data;
using CasinoRegistro.DataAccess.Data;
using CasinoRegistro.DataAccess.Data.Repository.IRepository;
using CasinoRegistro.DataAccess.Data.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using CasinoRegistro.DataAccess.Data.Initialiser;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("ConexionSQL") ?? throw new InvalidOperationException("String de Conexi�n 'ConexionSQL' no encontrada.");


////Context para Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// Agrego el contexto para las clases a usar
builder.Services.AddDbContext<CasinoRegistroDbContext>(options =>
options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultUI().AddDefaultTokenProviders();
builder.Services.AddControllersWithViews();



//Agregar contenedor de trabajo al contenedor IoC de inyecci�n de dependencias
builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();
//builder.Services.AddScoped<IInicializadorBD, InicializadorBD>();


//Siembra de datos - Paso 1
builder.Services.AddScoped<IInicializadorBD, InicializadorBD>();

////agrego para que que tome los datos configurados por defecto en el appsettings.json
//builder.Configuration.GetSection("Config");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "es-AR", "es" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRequestLocalization("es-AR");
//var cultureInfo = new CultureInfo("es-AR");
//cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
//cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
//CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
//CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

//Siembra de datos - Paso 2 Metodo que ejecuta la siembra de datos
SiembraDatos();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Cajero}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();



//Funcionalidad método SiembraDeDatos();
void SiembraDatos()
{
    using (var scope = app.Services.CreateScope())
    {
        var inicializadorBD = scope.ServiceProvider.GetRequiredService<IInicializadorBD>();
        inicializadorBD.Inicializar();
    }
}