using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface ICountriesGetterServices
    {
        Task<List<CountryResponse>> GetAllCountries();

        Task<CountryResponse?> GetCountryByCountryId(Guid? guid);
    }
}