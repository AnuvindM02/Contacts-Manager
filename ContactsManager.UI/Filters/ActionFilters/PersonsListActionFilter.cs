using CRUDXunitTest.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace ContactsManager.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method",nameof(PersonsListActionFilter),
                nameof(OnActionExecuted));

            //Accessing Viewdata

            PersonController personController = (PersonController)context.Controller;

            IDictionary<string, object?>? parameters = (IDictionary<string, object?>)context.HttpContext.Items["arguments"];

            if (parameters != null)
            {
                if (parameters.ContainsKey("searchBy"))
                {
                    personController.ViewData["CurrentSearchBy"] = Convert.ToString(parameters["searchBy"]);
                }

                if (parameters.ContainsKey("searchString"))
                {
                    personController.ViewData["CurrentSearchString"] = Convert.ToString(parameters["searchString"]);
                }

                if (parameters.ContainsKey("sortBy"))
                {
                    personController.ViewData["CurrentSortBy"] = Convert.ToString(parameters["sortBy"]);
                }

                if (parameters.ContainsKey("sortOrder"))
                {
                    personController.ViewData["CurrentSortOrder"] = Convert.ToString(parameters["sortOrder"]);
                }
            }

            personController.ViewBag.SearchFields = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName), "Person Name" },
                { nameof(PersonResponse.Email), "Email" },
                { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
                { nameof(PersonResponse.Gender), "Gender" },
                { nameof(PersonResponse.CountryID), "Country" },
                { nameof(PersonResponse.Address), "Address" }
            };

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method", nameof(PersonsListActionFilter),
                            nameof(OnActionExecuting));
            //To send arguments to OnActionExecuted Method above
            context.HttpContext.Items["arguments"] = context.ActionArguments;

            //Validating action method parameter
            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if(!string.IsNullOrEmpty(searchBy))
                {
                    var searchByOptions = new List<string>() { nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Gender),
                        nameof(PersonResponse.Address),
                        nameof(PersonResponse.CountryID),
                        nameof(PersonResponse.Email),
                        nameof(PersonResponse.DateOfBirth)};

                    if (searchByOptions.Any(temp => temp == searchBy) == false)
                    { 
                        //if searchBy have an unidentified value
                        _logger.LogInformation("searchBy actual value: {searchBy}", searchBy);

                        //searchBy is changed to PersonName as default
                        context.ActionArguments["searchBy"] = nameof(PersonResponse.PersonName);
                        searchBy = Convert.ToString(context.ActionArguments["searchBy"]);
                        _logger.LogInformation("searchBy updated value: {searchBy}", searchBy);
                    }
                }                
            }
        }
    }
}
