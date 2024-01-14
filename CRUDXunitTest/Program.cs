using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;
using RepositoryContracts;
using Repositories;
using Serilog;
using Serilog.Sinks;

var builder = WebApplication.CreateBuilder(args);

//Serilog
builder.Host.UseSerilog((HostBuilderContext context,IServiceProvider services,
    LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services);
});


builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountriesServices, CountriesServices>();
builder.Services.AddScoped<IPersonsServices, PersonsServices>();

builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});
var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
if (builder.Environment.IsEnvironment("Test") == false)
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers(); 

app.Run();

public partial class Program { }