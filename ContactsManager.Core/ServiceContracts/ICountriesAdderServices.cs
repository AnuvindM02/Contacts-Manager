using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface ICountriesAdderServices
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);

    }
}