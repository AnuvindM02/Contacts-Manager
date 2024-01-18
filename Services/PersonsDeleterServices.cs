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
    public class PersonsDeleterServices : IPersonsDeleterServices
    {
        private readonly IPersonsRepository _personsRepository;
        private readonly ILogger<PersonsGetterServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public PersonsDeleterServices(IPersonsRepository personsDbContext, ILogger<PersonsGetterServices> logger,
            IDiagnosticContext diagnosticContext)
        {
            _personsRepository = personsDbContext;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public async Task<bool> DeletePerson(Guid? PersonId)
        {
            if (PersonId == null)
                throw new ArgumentNullException(nameof(PersonId));

            Person? person = await _personsRepository.GetPersonByPersonId(PersonId.Value);
            if (person == null)
                return false;

            bool isDeleted = await _personsRepository.DeletePersonById(PersonId.Value);

            return isDeleted;

        }

        
    }
}
