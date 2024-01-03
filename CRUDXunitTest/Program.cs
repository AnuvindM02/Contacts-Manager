using ServiceContracts;
using Services;
using Microsoft.EntityFrameworkCore;
using Entities;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICountriesServices, CountriesServices>();
builder.Services.AddScoped<IPersonsServices, PersonsServices>();
builder.Services.AddDbContext<PersonsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});
var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
//Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PersonsDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", wkhtmltopdfRelativePath: "Rotativa");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers(); 

app.Run();
