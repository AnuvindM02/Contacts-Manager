
using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
/*using System.Threading.Tasks;
*/
namespace CRUDXTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesServices _countriesServices;

        public CountriesServiceTest()
        {
            _countriesServices = new CountriesServices(new PersonsDbContext
                (new DbContextOptionsBuilder<PersonsDbContext>().Options));
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;


            //Assert     If no argument supplied exception
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>  
            await _countriesServices.AddCountry(request));    //Act
        }


        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };


            //Assert       Some value is wrong in the argument exception
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            await _countriesServices.AddCountry(request));    //Act
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };


            //Assert
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                //Act
                await _countriesServices.AddCountry(request1);
                await _countriesServices.AddCountry(request2);
            });    
        }


        [Fact]
        public async Task AddCountry_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "Japan" };


            //Act

            CountryResponse response = await _countriesServices.AddCountry(request1);
            List<CountryResponse> countries_from_getcountrymethod = await _countriesServices.GetAllCountries();

            //Assert
            Assert.True(response.CountryId != Guid.Empty);
            Assert.Contains(response, countries_from_getcountrymethod);


        }

        #endregion

        #region GetAllCountries

        [Fact]
        //List should be empty by default

        public async Task GetAllCountries_EmptyList()
        {
            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesServices.GetAllCountries();

            //Assert
            Assert.Empty(actual_country_response_list);
        }

        [Fact]

        public async Task GetAllCountries_AddFewCountries()
        {
            List<CountryAddRequest> country_request_list = new List<CountryAddRequest>() {
            new CountryAddRequest() { CountryName = "India"},
            new CountryAddRequest() { CountryName = "Nepal"}
            };

            List<CountryResponse> countryresponse_list = new List<CountryResponse>();

            foreach (CountryAddRequest country_request in country_request_list)
            {
                CountryResponse country_response = await _countriesServices.AddCountry(country_request);
                countryresponse_list.Add(country_response);
            }

            List<CountryResponse> actualCountryResponseList = await _countriesServices.GetAllCountries();

            foreach(CountryResponse expected_country in countryresponse_list)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }
        }

        #endregion

        #region GetCountryById

        [Fact]
        public async Task GetCountryByCountryId_NullCountry()
        {
            Guid? countryId = null;
            CountryResponse? response = await _countriesServices.GetCountryByCountryId(countryId);
            Assert.Null(response);
        }

        [Fact]

        public async Task GetCountryByCountryId_ValidCountryId()
        {
            CountryAddRequest country_request = new CountryAddRequest { CountryName="India"};
            CountryResponse country_response = await _countriesServices.AddCountry(country_request);
            CountryResponse? country_response_from_getByCountryId=await _countriesServices.GetCountryByCountryId(country_response.CountryId);
            Assert.Equal(country_response, country_response_from_getByCountryId);
        }

        #endregion


    }
}
