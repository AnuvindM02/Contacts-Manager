﻿using ContactsManager.Filters;
using ContactsManager.Filters.ActionFilters;
using ContactsManager.Filters.AuthorizationFilter;
using ContactsManager.Filters.ExceptionFilter;
using ContactsManager.Filters.ResourceFilters;
using ContactsManager.Filters.ResultFilters;
using Entities;
using Entities.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;

namespace CRUDXunitTest.Controllers
{
    [Route("[controller]")]
    [TypeFilter(typeof(HandleExceptionFilter))]
    [TypeFilter(typeof(PersonsAlwaysRunResultFilter))] //To test if action methods with skip filter attribute skip this
    [ResponseHeaderFilterFactory("X-Key","X-Value")]
    public class PersonController : Controller
    {
        private readonly ICountriesGetterServices _countriesService;
        private readonly IPersonsGetterServices _personsGetterService;
        private readonly IPersonsAdderServices _personsAdderService;
        private readonly IPersonsDeleterServices _personsDeleterService;
        private readonly IPersonsSorterServices _personsSorterService;
        private readonly IPersonsUpdaterServices _personsUpdaterService;

        private readonly ILogger<PersonController> _logger;

        public PersonController(ICountriesGetterServices countriesService, IPersonsGetterServices personsGetterService,IPersonsAdderServices personsAdderServices,IPersonsDeleterServices personsDeleterServices,
            IPersonsSorterServices personsSorterServices,IPersonsUpdaterServices personsUpdaterServices,ILogger<PersonController> logger)
        {
            _countriesService = countriesService;
            _personsGetterService = personsGetterService;
            _personsAdderService = personsAdderServices;
            _personsDeleterService = personsDeleterServices;
            _personsSorterService = personsSorterServices;
            _personsUpdaterService = personsUpdaterServices;
            _logger = logger;
        }

        [ServiceFilter(typeof(PersonsListActionFilter))]
        [ResponseHeaderFilterFactory("My-Key", "My-Value")]
        [TypeFilter(typeof(PersonsListResultFilter))]
        [SkipFilter]//Ignores filters, the if condition should be implemented in the intended filter to skip
        [Route("[action]")]
        [Route("/")]
        public async Task<IActionResult> Index(string searchBy, string? searchString, string sortBy = nameof(PersonResponse.PersonName),SortOrderOptions sortOrder = SortOrderOptions.ASC)
        {
            _logger.LogInformation("Index action method of PersonsController");

            //Search Operation
            List<PersonResponse> persons = await _personsGetterService.GetFilteredPersons(searchBy, searchString);
            
            //Order Operation
            List<PersonResponse> sortedPersons = await _personsSorterService.GetSortedPersons(persons, sortBy, sortOrder);
            
            return View(sortedPersons);
        }

        [HttpGet]
        [Route("[action]")]
        [ResponseHeaderFilterFactory("Y-Key","Y-Value")]

        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp=>
            new SelectListItem() { Text=temp.CountryName, Value=temp.CountryId.ToString() });

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        [TypeFilter(typeof(PersonCreateEditActionFilter))]
        [TypeFilter(typeof(FeatureDisabledResourceFilter),Arguments =new object[] {false})]
        public async Task<IActionResult> Create(PersonAddRequest personRequest)
        {
            //call the service method
            PersonResponse personResponse = await _personsAdderService.AddPerson(personRequest);

            //navigate to Index() action method (it makes another get request to "persons/index")
            return RedirectToAction("Index", "Person");
        }

        [HttpGet]
        [Route("[action]/{PersonID}")]
        [TypeFilter(typeof(TokenResultFilter))]
        public async Task<IActionResult> Edit(Guid PersonID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(PersonID);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(temp =>
            new SelectListItem() { Text = temp.CountryName, Value = temp.CountryId.ToString() });

            return View(personUpdateRequest);

        }

        [HttpPost]
        [Route("[action]/{PersonId}")]
        [TypeFilter(typeof(PersonCreateEditActionFilter))]
        [TypeFilter(typeof(TokenAuthorizationFilter))]
        public async Task<IActionResult> Edit(PersonUpdateRequest personRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personRequest.PersonID);

            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            //Filter handles the invalid model state(and it won't reach this action method),if valid, code below works
                PersonResponse updatedPerson = await _personsUpdaterService.UpdatePerson(personRequest);
                return RedirectToAction("Index");
            
        }

        [HttpGet]
        [Route("[action]/{PersonID}")]
        public async Task<IActionResult> Delete(Guid PersonID)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(PersonID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

            return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personId}")]
        public async Task<IActionResult> Delete(PersonUpdateRequest personUpdateRequest)
        {
            PersonResponse? personResponse = await _personsGetterService.GetPersonByPersonId(personUpdateRequest.PersonID);
            if (personResponse == null)
                return RedirectToAction("Index");

            await _personsDeleterService.DeletePerson(personUpdateRequest.PersonID);
            return RedirectToAction("Index");

        }

        [Route("PersonPDF")]
        public async Task<IActionResult> PersonPDF()
        {
            //Get list of persons
            List<PersonResponse> persons = await _personsGetterService.GetAllPersons();

            //Return view as pdf
            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
            };
        }

        [Route("PersonCSV")]
        public async Task<IActionResult> PersonCSV()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream", "persons.csv");
        }

        [Route("PersonExcel")]
        public async Task<IActionResult> PersonExcel()
        {
            MemoryStream memoryStream = await _personsGetterService.GetPersonsExcelSheet();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
        }
    }
}
