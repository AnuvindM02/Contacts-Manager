using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using RepositoryContracts;

namespace Services
{
    public class CountriesGetterServices : ICountriesGetterServices
    {

        private readonly ICountriesRepository _countriesRepository;

        public CountriesGetterServices(ICountriesRepository countriesRepository)
        {

            _countriesRepository = countriesRepository;

        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            Country? countryfromrepo = _countriesRepository.GetCountryByName(countryAddRequest.CountryName);

            if (countryfromrepo!=null)
            {
                throw new ArgumentException("Country name already exists!");
            }


            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();


            _countriesRepository.AddCountry(country);

            return country.ToCountryResponse();
            
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {

            List<Country> countries = await _countriesRepository.GetAllCountries();
            return countries
              .Select(country => country.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? guid)
        {
            if (guid == null)
                return null;

            Country? country = await _countriesRepository.GetCountryById(guid.Value);
            if (country == null)
                return null;

            return country.ToCountryResponse();

        }

        public async Task<int> UploadCountriesFromExcelFile(IFormFile formFile)
        {
            MemoryStream stream = new MemoryStream();
            await formFile.CopyToAsync(stream);

            using (ExcelPackage excelPackage = new ExcelPackage(stream))
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets["Countries"];

                int rowCount = worksheet.Dimension.Rows;
                int countriesInserted = 0;
                for (int i = 2; i <= rowCount; i++)
                {
                    string? cellValue = Convert.ToString(worksheet.Cells[i, 1].Value);

                    if (cellValue != null)
                    {
                        string countryName = cellValue;
                        if (_countriesRepository.GetCountryByName(countryName) == null)
                        {
                            Country country= new Country() { CountryName = countryName };
                            _countriesRepository.AddCountry(country);
                            countriesInserted++;
                        }
                    }
                }
                return countriesInserted;
            }
        }
    }
}