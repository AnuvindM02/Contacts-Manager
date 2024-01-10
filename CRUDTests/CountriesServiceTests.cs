
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
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
            var countriesInitialData = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);

            _countriesServices = new CountriesServices(dbContext);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry()
        {
            //Arrange
            CountryAddRequest? request = null;


            //Assert     If no argument supplied exception
            Func<Task> action = (async () =>  
            await _countriesServices.AddCountry(request));
            
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task AddCountry_NullCountryName()
        {
            //Arrange
            CountryAddRequest? request = new CountryAddRequest() { CountryName = null };


            //Assert       Some value is wrong in the argument exception
            Func<Task> action = (async () =>
            await _countriesServices.AddCountry(request));

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName()
        {
            //Arrange
            CountryAddRequest? request1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest? request2 = new CountryAddRequest() { CountryName = "USA" };


            await _countriesServices.AddCountry(request1);
            //Assert
            Func<Task> action = (async() =>
            { 
                await _countriesServices.AddCountry(request2);
            });

            await action.Should().ThrowAsync<ArgumentException>();
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
            response.CountryId.Should().NotBe(Guid.Empty);
            countries_from_getcountrymethod.Should().Contain(response);


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
            actual_country_response_list.Should().BeEmpty();
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

            /*foreach(CountryResponse expected_country in countryresponse_list)
            {
                Assert.Contains(expected_country, actualCountryResponseList);
            }*/

            actualCountryResponseList.Should().BeEquivalentTo(countryresponse_list);
        }

        #endregion

        #region GetCountryById

        [Fact]
        public async Task GetCountryByCountryId_NullCountry()
        {
            Guid? countryId = null;
            CountryResponse? response = await _countriesServices.GetCountryByCountryId(countryId);
            response.Should().BeNull();
        }

        [Fact]

        public async Task GetCountryByCountryId_ValidCountryId()
        {
            CountryAddRequest country_request = new CountryAddRequest { CountryName="India"};
            CountryResponse country_response = await _countriesServices.AddCountry(country_request);
            CountryResponse? country_response_from_getByCountryId=await _countriesServices.GetCountryByCountryId(country_response.CountryId);
            country_response_from_getByCountryId.Should().Be(country_response);
        }

        #endregion


    }
}
