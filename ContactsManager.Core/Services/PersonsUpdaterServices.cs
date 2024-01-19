using Entities;
using Entities.Enums;
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
    public class PersonsUpdaterServices : IPersonsUpdaterServices
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsUpdaterServices(IPersonsRepository personsDbContext, ILogger<PersonsGetterServices> logger,
            IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsDbContext;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest)
        {
            if (personUpdateRequest == null)
                throw new ArgumentNullException(nameof(personUpdateRequest));

            // Validation
            ValidationHelper.ModelValidation(personUpdateRequest);

            // Convert PersonUpdateRequest to Person
            Person person = personUpdateRequest.ToPerson();

            if (person == null)
                return null;

            // Update Person
            Person updatedPerson = await _personsRepository.UpdatePerson(person);

            return updatedPerson.ToPersonResponse();
        }


    }
}
