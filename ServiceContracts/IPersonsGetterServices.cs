using Entities.Enums;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsGetterServices
    {
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonId(Guid? PersonId);
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
        /// <summary>
        /// Returns persons as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV data</returns>
        Task<MemoryStream> GetPersonsCSV();
        Task<MemoryStream> GetPersonsExcelSheet();
    }
}
