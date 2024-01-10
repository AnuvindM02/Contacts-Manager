using ServiceContracts;
using ServiceContracts.DTO;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace Services
{
    public class CountriesServices : ICountriesServices
    {

        private readonly ApplicationDbContext _db;

        public CountriesServices(ApplicationDbContext personsDbContext)
        {

            _db = personsDbContext;

        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if(countryAddRequest == null)
                throw new ArgumentNullException(nameof(countryAddRequest));

            if(countryAddRequest.CountryName == null)
                throw new ArgumentException(nameof(countryAddRequest.CountryName));

            if (_db.Countries.Any(country => country.CountryName == countryAddRequest.CountryName))
            {
                throw new ArgumentException("Country name already exists!");
            }


            Country country = countryAddRequest.ToCountry();
            country.CountryID = Guid.NewGuid();


            await _db.Countries.AddAsync(country);
            await _db.SaveChangesAsync();

            return country.ToCountryResponse();
            
        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            /*List<CountryResponse> country_response_list = new List<CountryResponse>();

            foreach(Country country in _countries)
            {
                country_response_list.Add(country.ToCountryResponse());
            }*/



            /*            return country_response_list;
            */
            return await _db.Countries.Select(country => country.ToCountryResponse()).ToListAsync();



        }

        public async Task<CountryResponse?> GetCountryByCountryId(Guid? guid)
        {
            if (guid == null)
                return null;

            Country? country = await _db.Countries.FirstOrDefaultAsync(country => country.CountryID == guid);

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
                        if (_db.Countries.Where(temp=>temp.CountryName == countryName).Count()==0)
                        {
                            CountryAddRequest countryAddRequest = new CountryAddRequest() { CountryName = countryName };
                            await AddCountry(countryAddRequest);
                            countriesInserted++;
                        }
                    }
                }
                return countriesInserted;
            }
        }
    }
}