using CRUDXunitTest.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;

namespace ContactsManager.Filters.ActionFilters
{
    public class PersonCreateEditActionFilter : IAsyncActionFilter
    {
        private readonly ICountriesServices _countriesServices;

        public PersonCreateEditActionFilter(ICountriesServices countriesServices)
        {
            _countriesServices = countriesServices;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //OnExecuting

            //To Access Model state
            if (context.Controller is PersonController personController)
            {
                if (!personController.ModelState.IsValid)
                {
                    List<CountryResponse> countries = await _countriesServices.GetAllCountries();
                    personController.ViewBag.Countries = countries.Select(temp =>
                                new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

                    personController.ViewBag.Errors = personController.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                    var personRequest = context.ActionArguments["personRequest"];
                    context.Result = personController.View(personRequest);//Short - circuit : skips subsequent action filters and methods 
                }
                else
                {
                    await next();
                }
            }


            //OnExecuted


        }
    }
}
