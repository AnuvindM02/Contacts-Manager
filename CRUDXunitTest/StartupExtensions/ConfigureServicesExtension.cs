using ContactsManager.Filters.ActionFilters;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repositories;
using RepositoryContracts;
using ServiceContracts;
using Services;

namespace ContactsManager
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddScoped<ICountriesServices, CountriesServices>();
            services.AddScoped<IPersonsServices, PersonsServices>();

            services.AddScoped<IPersonsRepository, PersonsRepository>();
            services.AddScoped<ICountriesRepository, CountriesRepository>();

            services.AddTransient<PersonsListActionFilter>();
            services.AddTransient<ResponseHeaderActionFilter>();//For IFilterFactory

            services.AddControllersWithViews(options =>
            {
                //Calling filter globally
                var logger = services.BuildServiceProvider()
                .GetRequiredService<ILogger<ResponseHeaderActionFilter>>();

                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    Key = "Global_Key",
                    Value = "Global_Value",
                });
            });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:DefaultConnection"]);
            });

            return services;
        }
    }
}
