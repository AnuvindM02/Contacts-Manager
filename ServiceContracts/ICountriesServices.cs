using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface ICountriesServices
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
        
        
        /// <returns>List of countries</returns>
        Task<List<CountryResponse>> GetAllCountries();


        Task<CountryResponse?> GetCountryByCountryId(Guid? guid);

        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}