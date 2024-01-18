
using AutoFixture;
using Entities;
using EntityFrameworkCoreMock;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
/*using System.Threading.Tasks;
*/
namespace CRUDXTests
{
    public class CountriesServiceTest
    {
        private readonly ICountriesGetterServices _countriesGetterServices;
        private readonly ICountriesAdderServices _countriesAdderServices;
        private readonly ICountriesUploaderServices _countriesUploaderServices;


        private readonly Mock<ICountriesRepository> _countriesRepositoryMock;
        private readonly ICountriesRepository _countriesRepository;
        private readonly IFixture _fixture;


        public CountriesServiceTest()
        {
            //Mocking DBContext
            /*var countriesInitialData = new List<Country>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);*/

            _fixture = new Fixture();

            _countriesRepositoryMock = new Mock<ICountriesRepository>();
            _countriesRepository = _countriesRepositoryMock.Object;
            _countriesGetterServices = new CountriesGetterServices(_countriesRepository);
            _countriesAdderServices = new CountriesAdderServices(_countriesRepository);
            _countriesUploaderServices = new CountriesUploaderServices(_countriesRepository);
        }

        #region AddCountry

        [Fact]
        public async Task AddCountry_NullCountry_ToBeArgumentNullException()
        {
            //Arrange
            CountryAddRequest? request = null;

            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock
            .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
            .Returns(country);

            //Assert     If no argument supplied exception
            Func<Task> action = async () =>  
            await _countriesAdderServices.AddCountry(request);
            
            await action.Should().ThrowAsync<ArgumentNullException>();
        }


        [Fact]
        public async Task AddCountry_NullCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest? request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, null as string)
                .Create();

            Country country = _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create();

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .Returns(country);

            //Assert       Some value is wrong in the argument exception
            Func<Task> action = (async () =>
            await _countriesAdderServices.AddCountry(request));

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddCountry_DuplicateCountryName_ToBeArgumentException()
        {
            //Arrange
            CountryAddRequest first_country_request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Test name").Create();
            CountryAddRequest second_country_request = _fixture.Build<CountryAddRequest>()
                .With(temp => temp.CountryName, "Test name").Create();

            Country first_country = first_country_request.ToCountry();
            Country second_country = second_country_request.ToCountry();

            _countriesRepositoryMock
                .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
                .Returns(first_country);

            //Return null when GetCountryByCountryName is called
            _countriesRepositoryMock
                .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
                .ReturnsAsync(null as Country);

            CountryResponse first_country_from_add_country = await _countriesAdderServices.AddCountry(first_country_request);

            //Act
            var action = async () =>
            {
                //Return first country when GetCountryByCountryName is called
                _countriesRepositoryMock.Setup(temp => temp.AddCountry(It.IsAny<Country>())).Returns(first_country);

                _countriesRepositoryMock.Setup(temp => temp.GetCountryByName(It.IsAny<string>())).ReturnsAsync(first_country);

                await _countriesAdderServices.AddCountry(second_country_request);
            };

            await action.Should().ThrowAsync<ArgumentException>();
        }


        [Fact]
        public async Task AddCountry_ProperCountryDetails_ProperCountryDetails()
        {
            //Arrange
            CountryAddRequest country_request = _fixture.Create<CountryAddRequest>();
            Country country = country_request.ToCountry();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.AddCountry(It.IsAny<Country>()))
             .Returns(country);

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryByName(It.IsAny<string>()))
             .ReturnsAsync(null as Country);


            //Act
            CountryResponse country_from_add_country = await _countriesAdderServices.AddCountry(country_request);

            country.CountryID = country_from_add_country.CountryId;
            country_response.CountryId = country_from_add_country.CountryId;

            //Assert
            country_from_add_country.CountryId.Should().NotBe(Guid.Empty);
            country_from_add_country.Should().BeEquivalentTo(country_response);
        }

        #endregion

        #region GetAllCountries

        [Fact]
        //List should be empty by default

        public async Task GetAllCountries_ToBeEmptyList()
        {
            List<Country> country_empty_list = new List<Country>();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_empty_list);

            //Acts
            List<CountryResponse> actual_country_response_list = await _countriesGetterServices.GetAllCountries();

            //Assert
            actual_country_response_list.Should().BeEmpty();
        }

        [Fact]

        public async Task GetAllCountries_AddFewCountries()
        {
            List<Country> country_list = new List<Country>() {
                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create(),
                _fixture.Build<Country>()
                .With(temp => temp.Persons, null as List<Person>).Create()
            };

            List<CountryResponse> country_response_list = country_list.Select(temp => temp.ToCountryResponse()).ToList();

            _countriesRepositoryMock.Setup(temp => temp.GetAllCountries()).ReturnsAsync(country_list);

            //Act
            List<CountryResponse> actualCountryResponseList = await _countriesGetterServices.GetAllCountries();

            //Assert
            actualCountryResponseList.Should().BeEquivalentTo(country_response_list);
        }

        #endregion

        #region GetCountryById

        [Fact]
        public async Task GetCountryByCountryId_NullCountry()
        {
            Guid? countryId = null;

            _countriesRepositoryMock
            .Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
            .ReturnsAsync(null as Country);

            CountryResponse? response = await _countriesGetterServices.GetCountryByCountryId(countryId);
            response.Should().BeNull();
        }

        [Fact]

        public async Task GetCountryByCountryId_ValidCountryId()
        {
            //Arrange
            Country country = _fixture.Build<Country>()
              .With(temp => temp.Persons, null as List<Person>)
              .Create();
            CountryResponse country_response = country.ToCountryResponse();

            _countriesRepositoryMock
             .Setup(temp => temp.GetCountryById(It.IsAny<Guid>()))
             .ReturnsAsync(country);

            //Act
            CountryResponse? country_response_from_get = await _countriesGetterServices.GetCountryByCountryId(country.CountryID);


            //Assert
            country_response_from_get.Should().Be(country_response);
        }

        #endregion


    }
}
