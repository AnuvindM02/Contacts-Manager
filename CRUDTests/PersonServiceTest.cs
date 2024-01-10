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

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonsServices _personsServices;
        private readonly ICountriesServices _countriesServices;
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IFixture _fixture;
        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _fixture = new Fixture();

            var countriesInitialData = new List<Country>() { };
            var personsInitialData = new List<Person>() { };

            DbContextMock<ApplicationDbContext> dbContextMock = new DbContextMock<ApplicationDbContext>(
                new DbContextOptionsBuilder<ApplicationDbContext>().Options);

            ApplicationDbContext dbContext = dbContextMock.Object;
            dbContextMock.CreateDbSetMock(temp => temp.Countries, countriesInitialData);
            dbContextMock.CreateDbSetMock(temp => temp.Persons, personsInitialData);

            _countriesServices = new CountriesServices(dbContext);
            _personsServices = new PersonsServices(dbContext, _countriesServices);

            //XUnit
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPersonAddRequest()
        {
            PersonAddRequest? personAddRequest = null;


            Func<Task> action = (async() =>
            {
                await _personsServices.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentNullException>();

        }


        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.PersonName, null as string)
                .Create();
            //Act
            Func<Task> action = (async() =>
            {
                await _personsServices.AddPerson(personAddRequest);
            });

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {
            /*PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                PersonName="Anuvind",Email="anuvindm02@gmail.com",Address="Avani Kizhakkepurakkal house",CountryID=Guid.NewGuid(),
                Gender=Gender.Male, DateOfBirth=DateTime.Parse("16-10-2002"),ReceiveNewsLetters=true
            };
            PersonAddRequest? personAddRequest2 = new PersonAddRequest()
            {
                PersonName = "Anuvidfnd",
                Email = "anuvindm02af@gmail.com",
                Address = "Avani Kizhaakkepurakkal house",
                CountryID = Guid.NewGuid(),
                Gender = Gender.Male,
                DateOfBirth = DateTime.Parse("16-04-2002"),
                ReceiveNewsLetters = false
            };*/

            //Using AutoFixture to create dummy person
            PersonAddRequest? personAddRequest =
                _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "anuvindm21@gmail.com")//As dummy email won't be validated
                .Create();

            PersonResponse person_response_from_add = await _personsServices.AddPerson(personAddRequest);

            List<PersonResponse> persons_list = await _personsServices.GetAllPersons();

            //Assert.True(person_response_from_add.PersonID != Guid.Empty);

            person_response_from_add.PersonID.Should().NotBe(Guid.Empty);


            persons_list.Should().Contain(person_response_from_add);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId()
        {
            Guid? PersonId = null;

            PersonResponse? person_response = await _personsServices.GetPersonByPersonId(PersonId);
            person_response.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonByPersonId_WithPersonId()
        {
            CountryAddRequest country_request = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse country_response = await _countriesServices.AddCountry(country_request);
            PersonAddRequest person_request = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someone@gmail.com")
                .Create();

            PersonResponse person_response_from_add = await _personsServices.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personsServices.GetPersonByPersonId(person_response_from_add.PersonID);

            //Assert
            person_response_from_get.Should().Be(person_response_from_add);
        }

        #endregion

        #region GetAllPersons

        [Fact]
        public async Task GetAllPersons_EmptyList()
        {
            Assert.Empty(await _personsServices.GetAllPersons());
        }

        [Fact]
        public async Task GetAllPersons_AddFewPersons()
        {
            CountryAddRequest countryReq1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryReq1);

            CountryAddRequest countryReq2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse2 = await _countriesServices.AddCountry(countryReq2);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone1@gmail.com").Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone2@gmail.com").Create()
            };

            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse personResponse = await _personsServices.AddPerson(personAddRequest);
                personResponses.Add(personResponse);
            }

            //test helper expected output
            _testOutputHelper.WriteLine("Expected output:");
            foreach (PersonResponse personResponse in personResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponse_list = await _personsServices.GetAllPersons();

            //test helper actual output
            _testOutputHelper.WriteLine("Actual output:");
            foreach (PersonResponse actual_person_response in actualPersonResponse_list)
            {
                _testOutputHelper.WriteLine(actual_person_response.ToString());
            }


            //Assert
            /*foreach (PersonResponse expectedPerson in personResponses)
            {
                Assert.Contains(expectedPerson, actualPersonResponse_list);
            }*/

            actualPersonResponse_list.Should().BeEquivalentTo(personResponses);

        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            CountryAddRequest countryReq1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryReq1);

            CountryAddRequest countryReq2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse2 = await _countriesServices.AddCountry(countryReq2);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone1@gmail.com").Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone2@gmail.com").Create()
            };

            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse personResponse = await _personsServices.AddPerson(personAddRequest);
                personResponses.Add(personResponse);
            }

            //test helper expected output
            _testOutputHelper.WriteLine("Expected output:");
            foreach (PersonResponse personResponse in personResponses)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            List<PersonResponse> actualPersonResponse_listFromSearch = await _personsServices.GetFilteredPersons(nameof(Person.PersonName),"");

            //test helper actual output
            _testOutputHelper.WriteLine("Actual output:");
            foreach (PersonResponse actual_person_response in actualPersonResponse_listFromSearch)
            {
                _testOutputHelper.WriteLine(actual_person_response.ToString());
            }


            //Assert
            /*foreach (PersonResponse expectedPerson in personResponses)
            {
                Assert.Contains(expectedPerson, actualPersonResponse_listFromSearch);
            }*/

            actualPersonResponse_listFromSearch.Should().BeEquivalentTo(personResponses);

        }

        [Fact]
        public async Task GetFilteredPersons_ByPersonNameSearch()
        {

            CountryAddRequest countryReq1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryReq1);

            CountryAddRequest countryReq2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse2 = await _countriesServices.AddCountry(countryReq2);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"someone1@gmail.com")
                .With(temp => temp.PersonName, "Vishal").Create(),
                _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"someone2@gmail.com")
                .With(temp => temp.PersonName, "Kushal").Create(),
                _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email,"someone3@gmail.com")
                .With(temp => temp.PersonName, "Vishakh").Create()
            };

            List<PersonResponse> personResponses = new List<PersonResponse>();

            foreach (PersonAddRequest personAddRequest in personAddRequests_list)
            {
                PersonResponse personResponse = await _personsServices.AddPerson(personAddRequest);
                personResponses.Add(personResponse);
            }

            string searchString = "vi";

            //test helper expected output
            _testOutputHelper.WriteLine("Expected output:");
            foreach (PersonResponse personResponse in personResponses)
            {
                if (personResponse.PersonName != null)
                {
                    if (personResponse.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    {
                        _testOutputHelper.WriteLine(personResponse.ToString());
                    }
                }
            }

            List<PersonResponse> personresponseListFiltered = await _personsServices.GetFilteredPersons(nameof(Person.PersonName), searchString);

            //test helper actual output
            _testOutputHelper.WriteLine("Actual output:");
            foreach(PersonResponse personResponse in personresponseListFiltered)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            //Assert
            /*foreach(PersonResponse personResponseFromAdd in personResponses)
            {
                if (personResponseFromAdd.PersonName != null)
                {
                    if (personResponseFromAdd.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personResponseFromAdd,personresponseListFiltered);
                    }
                }
            }*/

            personresponseListFiltered.Should().OnlyContain(person => person.PersonName.Contains("vi", StringComparison.OrdinalIgnoreCase));


        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest countryReq1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse1 = await _countriesServices.AddCountry(countryReq1);

            CountryAddRequest countryReq2 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse countryResponse2 = await _countriesServices.AddCountry(countryReq2);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone1@gmail.com").Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone2@gmail.com").Create(),
                _fixture.Build<PersonAddRequest>().With(temp=>temp.Email,"someone2@gmail.com").Create()
            };


            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in personAddRequests_list)
            {
                PersonResponse person_response =await _personsServices.AddPerson(person_request);
                person_response_list_from_add.Add(person_response);
            }

            _testOutputHelper.WriteLine("Expected output:");
            person_response_list_from_add = person_response_list_from_add.OrderByDescending(temp => temp.PersonName).ToList();
            foreach(PersonResponse personResponse in person_response_list_from_add)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            _testOutputHelper.WriteLine("Actual output:");
            List<PersonResponse> allPersons = await _personsServices.GetAllPersons();
            List<PersonResponse> person_response_list_from_sort = _personsServices.GetSortedPersons(allPersons,nameof(Person.PersonName),SortOrderOptions.DESC);

            foreach(PersonResponse personResponse in person_response_list_from_sort)
            {
                _testOutputHelper.WriteLine(personResponse.ToString());
            }

            /*for(int i=0;i< person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_response_list_from_sort[i]);
            }*/

            person_response_list_from_sort.Should().BeInDescendingOrder(person => person.PersonName);
        }

        #endregion

        #region PersonUpdateRequest

        [Fact]
        public async Task PersonUpdateRequest_NullPersonUpdateRequest()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            Func<Task> action = (async() =>await _personsServices.UpdatePerson(personUpdateRequest));
            await action.Should().ThrowAsync<ArgumentNullException>();

        }

        [Fact]
        public async Task PersonUpdateRequest_InvalidPersonId()
        {
            PersonUpdateRequest? personUpdateRequest = _fixture.Build<PersonUpdateRequest>().Create();

            Func<Task> action = (async() =>
            {
                await _personsServices.UpdatePerson(personUpdateRequest);
            });

            await action.Should().ThrowAsync<ArgumentException>();
        }

        [Fact]
        public async Task PersonUpdateRequest_NullPersonName()
        {
            CountryAddRequest country_request_1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp=>temp.Email, "someemail@gmail.com").Create();
            PersonResponse personResponse = await _personsServices.AddPerson(person_request_1);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            Func<Task> action = (async() => 
            {
                await _personsServices.UpdatePerson(personUpdateRequest);
            });

            await action.Should().ThrowAsync<ArgumentException>();

        }

        [Fact]
        public async Task PersonUpdateRequest_ProperDetails()
        {
            CountryAddRequest country_request_1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someemail@gmail.com").Create();
            PersonResponse personResponse = await _personsServices.AddPerson(person_request_1);


            _testOutputHelper.WriteLine("Original person before updation:");
            _testOutputHelper.WriteLine(personResponse.ToString());


            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = "NewName";
            personUpdateRequest.Email = "newname@gmail.com";

            PersonResponse person_response_from_update = await _personsServices.UpdatePerson(personUpdateRequest);
            PersonResponse? person_response_from_get = await _personsServices.GetPersonByPersonId(person_response_from_update.PersonID);

            _testOutputHelper.WriteLine("After updation:");
            _testOutputHelper.WriteLine($"From person_response from update:{person_response_from_update.ToString()}");
            _testOutputHelper.WriteLine($"From person_response from get:{person_response_from_get.ToString()}");

            person_response_from_update.Should().Be(person_response_from_get);


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
        public async Task DeletePersons_ProperPersonId()
        {
            CountryAddRequest country_request_1 = _fixture.Build<CountryAddRequest>().Create();
            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            PersonAddRequest person_request_1 = _fixture.Build<PersonAddRequest>()
                .With(temp => temp.Email, "someemail@gmail.com").Create();
            PersonResponse personResponse = await _personsServices.AddPerson(person_request_1);


            bool isDeleted = await _personsServices.DeletePerson(personResponse.PersonID);
            isDeleted.Should().BeTrue();
        }

        #endregion
    }
}