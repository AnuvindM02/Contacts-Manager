using Entities.Enums;
using ServiceContracts;
using ServiceContracts.DTO;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit.Abstractions;
using Entities;
using Microsoft.EntityFrameworkCore;
using EntityFrameworkCoreMock;
using AutoFixture;
using FluentAssertions;
using Moq;
using RepositoryContracts;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonsServices _personsServices;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;

        private readonly Mock<IPersonsRepository> _personsRepositoryMock;
        private readonly IPersonsRepository _personsRepository;

        private readonly ILogger<PersonsServices> _logger;
        private readonly IDiagnosticContext _diagnosticContext;


        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            //Mocking Repository
            _personsRepositoryMock = new Mock<IPersonsRepository>();
            _personsRepository = _personsRepositoryMock.Object;

            //Mocking DBContext
            /* var countriesInitialData = new List<Country>() { };
             var personsInitialData = new List<Person>() { };

             DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                 new DbContextOptionsBuilder<ApplicationDbContext>().Options);

             ApplicationDbContext dbContext = dbContextMock.Object;
             dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
             dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);*/
            var diagnosticContextMock = new Mock<IDiagnosticContext>();
            _diagnosticContext = diagnosticContextMock.Object;

            var logger = new Mock<ILogger<PersonsServices>>();
            _logger = logger.Object;

            _personsServices = new PersonsServices(_personsRepository,_logger,_diagnosticContext);
            //XUnit test output helper
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPerson_ToBeArgumentNullException()
        {
            PersonAddRequest? personAddRequest = null;


            Func<Task> action = (async() =>
            {
                await _personsServices.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentNullException>();

        }


        [Fact]
        public async Task AddPerson_NullPersonName_ToBeArgumentException()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();

            Person person = personAddRequest.ToPerson();

            _personsRepositoryMock
            .Setup(temp => temp.AddPerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

            //Act
            Func<Task> action = (async() =>
            {
                await _personsServices.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails_ToBeSuccessful()
        {
            //Using AutoFixture to create dummy person
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "anuvindm21@gmail.com")//As dummy email won't be validated
                .Create();

            Person person = personAddRequest.ToPerson();
            PersonResponse person_response_expected = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.AddPerson(It.IsAny<Person>()))
                .ReturnsAsync(person);


            PersonResponse person_response_from_add = await _personsServices.AddPerson(personAddRequest);
            person_response_expected.PersonID = person_response_from_add.PersonID;

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);


            person_response_from_add.Should().Be(person_response_expected);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId_ToBeNull()
        {
            Guid? PersonId = null;

            PersonResponse? person_response = await _personsServices.GetPersonByPersonId(PersonId);
            person_response.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonId_WithPersonId_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
                .With(temp => temp.Email, "someone@gmail.com")
                .With(temp => temp.Country, null as Country)
                .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();

            _personsRepositoryMock.Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
            .ReturnsAsync(person);

            PersonResponse? person_response_expected = await _personsServices.GetPersonByPersonId(person.PersonID);

            //Assert
            person_response_from_add.Should().Be(person_response_expected);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_ToBeEmptyList()
        {
            var persons = new List<Person>();

            _personsRepositoryMock
             .Setup(temp => temp.GetAllPersons())
             .ReturnsAsync(persons);

            List<PersonResponse> persons_from_get = await _personsServices.GetAllPersons();

            persons_from_get.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {            
            List<Person> persons= new List<Person>()
            {
                _fixture.Build<Person>().With(temp=>temp.Email,"someone1@gmail.com").With(temp => temp.Country, null as Country)
                .Create(),
                _fixture.Build<Person>().With(temp=>temp.Email,"someone2@gmail.com").With(temp => temp.Country, null as Country)
                .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            //test helper expected output
            _testOutputHelper.WriteLine("Expected output:");
            foreach (PersonResponse personResponse in person_response_list_expected)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            //Act
            List<PersonResponse> persons_list_from_get = await _personsServices.GetAllPersons();

            //test helper actual output
            _testOutputHelper.WriteLine("Actual output:");
            foreach (PersonResponse actual_person_response in persons_list_from_get)
            {
                _testOutputHelper.WriteLine(actual_person_response.ToString());
            }


            //Assert
            /*foreach (PersonResponse expectedPerson in personResponses)
            {
                Assert.Contains(expectedPerson, actualPersonResponse_list);
            }*/

            persons_list_from_get.Should().BeEquivalentTo(person_response_list_expected);

        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            //Arrange
            List<Person> persons = new List<Person>() {
            _fixture.Build<Person>()
            .With(temp => temp.Email, "someone_1@example.com")
            .With(temp => temp.Country, null as Country)
            .Create(),

            _fixture.Build<Person>()
            .With(temp => temp.Email, "someone_2@example.com")
            .With(temp => temp.Country, null as Country)
            .Create(),

            _fixture.Build<Person>()
            .With(temp => temp.Email, "someone_3@example.com")
            .With(temp => temp.Country, null as Country)
            .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();
            _personsRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

            _personsRepositoryMock.Setup(temp => temp.GetAllPersons()).ReturnsAsync(persons);

            List<PersonResponse> actualPersonResponse_listFromSearch = await _personsServices.GetFilteredPersons(nameof(Person.PersonName),"");

            actualPersonResponse_listFromSearch.Should().BeEquivalentTo(person_response_list_expected);

        }

        [Fact]
        public async Task GetFilteredPersons_ByPersonNameSearch()
        {
            List<Person> persons= new List<Person>
            {
                _fixture.Build<Person>()
                .With(temp=>temp.Email,"someone1@gmail.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.PersonName, "Vishal").Create(),
                _fixture.Build<Person>()
                .With(temp=>temp.Email,"someone2@gmail.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.PersonName, "Kushal").Create(),
                _fixture.Build<Person>()
                .With(temp=>temp.Email,"someone3@gmail.com")
                .With(temp => temp.Country, null as Country)
                .With(temp => temp.PersonName, "Vishakh").Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock.Setup(temp => temp
            .GetFilteredPersons(It.IsAny<Expression<Func<Person, bool>>>()))
            .ReturnsAsync(persons);

            string searchString = "vi";
            List<PersonResponse> personresponseListFiltered = await _personsServices.GetFilteredPersons(nameof(Person.PersonName), searchString);

            personresponseListFiltered.Should().BeEquivalentTo(person_response_list_expected);

        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons_ToBeSuccessful()
        {
            List<Person> persons = new List<Person>() {
            _fixture.Build<Person>()
            .With(temp => temp.Email, "someone_1@example.com")
            .With(temp => temp.Country, null as Country)
            .Create(),

            _fixture.Build<Person>()
            .With(temp => temp.Email, "someone_2@example.com")
            .With(temp => temp.Country, null as Country)
            .Create(),

            _fixture.Build<Person>()
            .With(temp => temp.Email, "someone_3@example.com")
            .With(temp => temp.Country, null as Country)
            .Create()
            };

            List<PersonResponse> person_response_list_expected = persons.Select(temp => temp.ToPersonResponse()).ToList();

            _personsRepositoryMock
            .Setup(temp => temp.GetAllPersons())
            .ReturnsAsync(persons);

            List<PersonResponse> allPersons = await _personsServices.GetAllPersons();

            
            //Act
            List<PersonResponse> persons_list_from_sort = await _personsServices.GetSortedPersons(allPersons, nameof(Person.PersonName), SortOrderOptions.DESC);

            persons_list_from_sort.Should().BeInDescendingOrder(person => person.PersonName);
        }

        #endregion

        #region PersonUpdateRequest

        [Fact]
        public async Task PersonUpdateRequest_NullPerson_ToBeArgumentNullException()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> action = (async() =>await _personsServices.UpdatePerson(personUpdateRequest));
            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        [Fact]
        public async Task PersonUpdateRequest_InvalidPersonId_ToBeArgumentException()
        {
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            Func<Task> action = (async() =>
            {
                await _personsServices.UpdatePerson(personUpdateRequest);
            });

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task PersonUpdateRequest_NullPersonName_ToBeArgumentException()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.PersonName, null as string)
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Male")
             .Create();

            PersonResponse person_response_from_add = person.ToPersonResponse();
            PersonUpdateRequest person_update_request = person_response_from_add.ToPersonUpdateRequest();

            Func<Task> action = (async() => 
            {
                await _personsServices.UpdatePerson(person_update_request);
            });

            await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task PersonUpdateRequest_ProperDetails_ToBeSuccessful()
        {
            Person person = _fixture.Build<Person>()
            .With(temp => temp.Email, "someone@example.com")
            .With(temp => temp.Country, null as Country)
            .With(temp => temp.Gender, "Male")
            .Create();

            PersonResponse person_response_expected = person.ToPersonResponse();

            PersonUpdateRequest person_update_request = person_response_expected.ToPersonUpdateRequest();

            _personsRepositoryMock
            .Setup(temp => temp.UpdatePerson(It.IsAny<Person>()))
            .ReturnsAsync(person);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            PersonResponse person_response_from_update = await _personsServices.UpdatePerson(person_update_request);

            person_response_from_update.Should().Be(person_response_expected);

        }
            #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePersons_InvalidPersonId()
        {
            Guid newGuid = Guid.NewGuid();
            bool isDeleted = await _personsServices.DeletePerson(newGuid);

            isDeleted.Should().BeFalse();
        }

        [Fact]
        public async Task DeletePersons_ProperPersonId_ToBeSuccessful()
        {
            //Arrange
            Person person = _fixture.Build<Person>()
             .With(temp => temp.Email, "someone@example.com")
             .With(temp => temp.Country, null as Country)
             .With(temp => temp.Gender, "Female")
             .Create();

            _personsRepositoryMock
            .Setup(temp => temp.DeletePersonById(It.IsAny<Guid>()))
            .ReturnsAsync(true);

            _personsRepositoryMock
             .Setup(temp => temp.GetPersonByPersonId(It.IsAny<Guid>()))
             .ReturnsAsync(person);

            bool isDeleted = await _personsServices.DeletePerson(person.PersonID);
            isDeleted.Should().BeTrue();
        }

        #endregion
    }
}