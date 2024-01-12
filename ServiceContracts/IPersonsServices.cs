using Entities.Enums;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsServices
    {
        Task<PersonResponse> AddPerson(PersonAddRequest? personAddRequest);
        Task<List<PersonResponse>> GetAllPersons();
        Task<PersonResponse?> GetPersonByPersonId(Guid? PersonId);
        Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString);
        Task <List<PersonResponse>> GetSortedPersons(List<PersonResponse> allPersons, string? sortBy, SortOrderOptions sortOrder);
        Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest);
        Task<bool> DeletePerson(Guid? PersonId);
        /// <summary>
        /// Returns persons as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV data</returns>
        Task<MemoryStream> GetPersonsCSV();

        Task<MemoryStream> GetPersonsExcelSheet();

    }
}
