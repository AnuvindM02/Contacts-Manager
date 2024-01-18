using Entities;
using Entities.Enums;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using Services.Helpers;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using OfficeOpenXml;
using RepositoryContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogTimings;

namespace Services
{
    public class PersonsAdderServices : IPersonsAdderServices
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsAdderServices(IPersonsRepository personsDbContext, ILogger<PersonsGetterServices> logger,
            IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsDbContext;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        /*private PersonResponse PersonResponseWithCountryAdded(Person person)
        {
            PersonResponse personResponse = person.ToPersonResponse();
            personResponse.Country = _countriesServices.GetCountryByCountryId(person.CountryID)?.CountryName;
            return personResponse;
        }*/

        public async Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest)
        {
            if (personAddRequest == null)
                throw new ArgumentNullException(nameof(personAddRequest));

            ValidationHelper.ModelValidation(personAddRequest);

            Person person = personAddRequest.ToPerson();
            person.PersonID = Guid.NewGuid();

            _personsRepository.AddPerson(person);

            return person.ToPersonResponse();
        }
    }
}
