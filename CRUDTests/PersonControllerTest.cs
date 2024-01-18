using AutoFixture;
using Entities.Enums;
using Moq;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRUDXunitTest.Controllers;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
namespace CRUDTests
{
    public class PersonControllerTest
    {
        private readonly IPersonsGetterServices _personsGetterService;
        private readonly IPersonsAdderServices _personsAdderService;
        private readonly IPersonsDeleterServices _personsDeleterService;
        private readonly IPersonsUpdaterServices _personsUpdaterService;
        private readonly IPersonsSorterServices _personsSorterService;

        private readonly ICountriesGetterServices _countriesService;
        private readonly ILogger<PersonController> _logger;

        private readonly Mock<ICountriesGetterServices> _countriesServiceMock;
        private readonly Mock<IPersonsGetterServices> _personsGetterServiceMock;
        private readonly Mock<IPersonsAdderServices> _personsAdderServiceMock;
        private readonly Mock<IPersonsDeleterServices> _personsDeleterServiceMock;
        private readonly Mock<IPersonsSorterServices> _personsSorterServiceMock;
        private readonly Mock<IPersonsUpdaterServices> _personsUpdaterServiceMock;

        private readonly Mock<ILogger<PersonController>> _personControllerLoggerMock;

        private readonly Fixture _fixture;

        public PersonControllerTest()
        {
            _fixture = new Fixture();

            _countriesServiceMock = new Mock<ICountriesGetterServices>();

            _personsGetterServiceMock = new Mock<IPersonsGetterServices>();
            _personsAdderServiceMock = new Mock<IPersonsAdderServices>();
            _personsDeleterServiceMock = new Mock<IPersonsDeleterServices>();
            _personsSorterServiceMock = new Mock<IPersonsSorterServices>();
            _personsUpdaterServiceMock = new Mock<IPersonsUpdaterServices>();


            _personControllerLoggerMock = new Mock<ILogger<PersonController>>();

            _countriesService = _countriesServiceMock.Object;
            _personsGetterService = _personsGetterServiceMock.Object;
            _personsAdderService = _personsAdderServiceMock.Object;
            _personsDeleterService = _personsDeleterServiceMock.Object;
            _personsSorterService = _personsSorterServiceMock.Object;
            _personsUpdaterService = _personsUpdaterServiceMock.Object;

            _logger = _personControllerLoggerMock.Object;
        }

        #region Index
        [Fact]
        public async Task Index_ShouldReturnIndexViewWithPersonsList()
        {
            //Arrange
            List<PersonResponse> persons_response_list = _fixture.Create<List<PersonResponse>>();

            PersonController personsController = new PersonController(_countriesService, _personsGetterService,
                            _personsAdderService, _personsDeleterService, _personsSorterService, _personsUpdaterService, _logger);
            _personsGetterServiceMock
             .Setup(temp => temp.GetFilteredPersons(It.IsAny<string>(), It.IsAny<string>()))
             .ReturnsAsync(persons_response_list);

            _personsSorterServiceMock
             .Setup(temp => temp.GetSortedPersons(It.IsAny<List<PersonResponse>>(), It.IsAny<string>(), It.IsAny<SortOrderOptions>()))
             .ReturnsAsync(persons_response_list);

            //Act
            IActionResult result = await personsController.Index(_fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<SortOrderOptions>());

            //Assert
            ViewResult viewResult = Assert.IsType<ViewResult>(result);

            viewResult.ViewData.Model.Should().BeAssignableTo<IEnumerable<PersonResponse>>();
            viewResult.ViewData.Model.Should().Be(persons_response_list);
        }
        #endregion


        #region Create

        [Fact]
        public async void Create_IfNoModelErrors_ToReturnRedirectToIndex()
        {
            //Arrange
            PersonAddRequest person_add_request = _fixture.Create<PersonAddRequest>();

            PersonResponse person_response = _fixture.Create<PersonResponse>();

            List<CountryResponse> countries = _fixture.Create<List<CountryResponse>>();

            _countriesServiceMock
             .Setup(temp => temp.GetAllCountries())
             .ReturnsAsync(countries);

            _personsAdderServiceMock
             .Setup(temp => temp.AddPerson(It.IsAny<PersonAddRequest>()))
            .ReturnsAsync(person_response);

            PersonController personsController = new PersonController(_countriesService, _personsGetterService,
                _personsAdderService,_personsDeleterService,_personsSorterService,_personsUpdaterService,_logger);


            //Act
            IActionResult result = await personsController.Create(person_add_request);

            //Assert
            RedirectToActionResult redirectResult = Assert.IsType<RedirectToActionResult>(result);

            redirectResult.ActionName.Should().Be("Index");
        }

        #endregion
    }
}
