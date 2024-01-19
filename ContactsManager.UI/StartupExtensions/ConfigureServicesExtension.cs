using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Filters.ActionFilters;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
            services.AddScoped<ICountriesGetterServices, CountriesGetterServices>();
            services.AddScoped<ICountriesAdderServices, CountriesAdderServices>();
            services.AddScoped<ICountriesUploaderServices, CountriesUploaderServices>();


            services.AddScoped<IPersonsGetterServices, PersonGetterServiceFewExcelFields>();
            //Injects in the PersonGetterServiceFewExcelFields class
            services.AddScoped<PersonsGetterServices, PersonsGetterServices>();
            services.AddScoped<IPersonsAdderServices, PersonsAdderServices>();
            services.AddScoped<IPersonsDeleterServices, PersonsDeleterServices>();
            services.AddScoped<IPersonsSorterServices, PersonsSorterServices>();
            services.AddScoped<IPersonsUpdaterServices, PersonsUpdaterServices>();


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
                options.EnableSensitiveDataLogging();
            });

            services.AddIdentity<ApplicationUser,ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser,ApplicationRole,ApplicationDbContext,Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole,ApplicationDbContext,Guid>>();

            return services;
        }
    }
}
