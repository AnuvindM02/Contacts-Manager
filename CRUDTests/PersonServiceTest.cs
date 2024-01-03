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

namespace CRUDTests
{
    public class PersonServiceTest
    {
        private readonly IPersonsServices _personsServices;
        private readonly ICountriesServices _countriesServices;
        private readonly ITestOutputHelper _testOutputHelper;

        public PersonServiceTest(ITestOutputHelper testOutputHelper)
        {
            _countriesServices = new CountriesServices(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options));
            _personsServices = new PersonsServices(new PersonsDbContext(new DbContextOptionsBuilder<PersonsDbContext>().Options),_countriesServices);
            _testOutputHelper = testOutputHelper;
        }

        #region AddPerson

        [Fact]
        public async Task AddPerson_NullPersonAddRequest()
        {
            PersonAddRequest? personAddRequest = null;


            await Assert.ThrowsAsync<ArgumentNullException>(async() =>
            {
                await _personsServices.AddPerson(personAddRequest);
            });

        }


        [Fact]
        public async Task AddPerson_NullPerson()
        {
            //Arrange
            PersonAddRequest? personAddRequest = new PersonAddRequest()
            {
                Email = "anuvindm02@gmail.com",
                Address = "Avani Kizhakkepurakkal house",
                CountryID = Guid.NewGuid(),
                Gender = Gender.Male,
                DateOfBirth = DateTime.Parse("16-10-2002"),
                ReceiveNewsLetters = true
            };
            //Act
            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personsServices.AddPerson(personAddRequest);
            });
        }

        [Fact]
        public async Task AddPerson_ProperPersonDetails()
        {           
            PersonAddRequest? personAddRequest = new PersonAddRequest()
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
            };

            PersonResponse person_response_from_add = await _personsServices.AddPerson(personAddRequest);
            PersonResponse person_response_from_add2 = await _personsServices.AddPerson(personAddRequest);

            List<PersonResponse> persons_list = await _personsServices.GetAllPersons();

            Assert.True(person_response_from_add.PersonID != Guid.Empty);
            Assert.True(person_response_from_add2.PersonID != Guid.Empty);
            Assert.Contains(person_response_from_add, persons_list);
            Assert.Contains(person_response_from_add2, persons_list);
        }

        #endregion

        #region GetPersonByPersonId

        [Fact]
        public async Task GetPersonByPersonId_NullPersonId()
        {
            Guid? PersonId = null;

            Assert.Null(await _personsServices.GetPersonByPersonId(PersonId));
        }

        [Fact]
        public async Task GetPersonByPersonId_WithPersonId()
        {
            CountryAddRequest country_request = new CountryAddRequest() { CountryName = "India" };
            CountryResponse country_response = await _countriesServices.AddCountry(country_request);
            PersonAddRequest person_request = new PersonAddRequest() { PersonName = "person name...", Email = "email@sample.com", Address = "address", CountryID = country_response.CountryId, DateOfBirth = DateTime.Parse("2000-01-01"), Gender = Gender.Male, ReceiveNewsLetters = false };

            PersonResponse person_response_from_add = await _personsServices.AddPerson(person_request);

            PersonResponse? person_response_from_get = await _personsServices.GetPersonByPersonId(person_response_from_add.PersonID);

            //Assert
            Assert.Equal(person_response_from_add, person_response_from_get);
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
            CountryAddRequest IndiaAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryResponse IndiaCountryResponse = await _countriesServices.AddCountry(IndiaAddRequest);

            CountryAddRequest USAAddRequest = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse USACountryResponse = await _countriesServices.AddCountry(USAAddRequest);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                new PersonAddRequest()
            {
                PersonName="Anuvind",Email="anuvindm02@gmail.com",Address="Avani Kizhakkepurakkal house",CountryID=IndiaCountryResponse.CountryId,
                Gender=Gender.Male, DateOfBirth=DateTime.Parse("16-10-2002"),ReceiveNewsLetters=true
            },
                new PersonAddRequest()
            {
                PersonName = "Anuvidfnd",
                Email = "anuvindm02af@gmail.com",
                Address = "Avani Kizhaakkepurakkal house",
                CountryID = USACountryResponse.CountryId,
                Gender = Gender.Male,
                DateOfBirth = DateTime.Parse("16-04-2002"),
                ReceiveNewsLetters = false
            }

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
            foreach (PersonResponse expectedPerson in personResponses)
            {
                Assert.Contains(expectedPerson, actualPersonResponse_list);
            }

        }
        #endregion

        #region GetFilteredPersons

        [Fact]
        public async Task GetFilteredPersons_EmptySearchString()
        {
            CountryAddRequest IndiaAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryResponse IndiaCountryResponse = await _countriesServices.AddCountry(IndiaAddRequest);

            CountryAddRequest USAAddRequest = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse USACountryResponse = await _countriesServices.AddCountry(USAAddRequest);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                new PersonAddRequest()
            {
                PersonName="Anuvind",Email="anuvindm02@gmail.com",Address="Avani Kizhakkepurakkal house",CountryID=IndiaCountryResponse.CountryId,
                Gender=Gender.Male, DateOfBirth=DateTime.Parse("16-10-2002"),ReceiveNewsLetters=true
            },
                new PersonAddRequest()
            {
                PersonName = "Anuvidfnd",
                Email = "anuvindm02af@gmail.com",
                Address = "Avani Kizhaakkepurakkal house",
                CountryID = USACountryResponse.CountryId,
                Gender = Gender.Male,
                DateOfBirth = DateTime.Parse("16-04-2002"),
                ReceiveNewsLetters = false
            }

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
            foreach (PersonResponse expectedPerson in personResponses)
            {
                Assert.Contains(expectedPerson, actualPersonResponse_listFromSearch);
            }

        }

        [Fact]
        public async Task GetFilteredPersons_ByPersonNameSearch()
        {

            CountryAddRequest IndiaAddRequest = new CountryAddRequest() { CountryName = "India" };
            CountryResponse IndiaCountryResponse = await _countriesServices.AddCountry(IndiaAddRequest);

            CountryAddRequest USAAddRequest = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse USACountryResponse = await _countriesServices.AddCountry(USAAddRequest);

            List<PersonAddRequest> personAddRequests_list = new List<PersonAddRequest>
            {
                new PersonAddRequest()
            {
                PersonName="Anuvind",Email="anuvindm02@gmail.com",Address="Avani Kizhakkepurakkal house",CountryID=IndiaCountryResponse.CountryId,
                Gender=Gender.Male, DateOfBirth=DateTime.Parse("16-10-2002"),ReceiveNewsLetters=true
            },
                new PersonAddRequest()
            {
                PersonName = "Athul",
                Email = "athul@gmail.com",
                Address = "Avani Kizhaakkepurakkal house",
                CountryID = USACountryResponse.CountryId,
                Gender = Gender.Male,
                DateOfBirth = DateTime.Parse("16-04-2002"),
                ReceiveNewsLetters = false
            },
                new PersonAddRequest()
            {
                PersonName = "Vishal",
                Email = "vishal@gmail.com",
                Address = "Avani Kizhaakkepurakkal house",
                CountryID = USACountryResponse.CountryId,
                Gender = Gender.Male,
                DateOfBirth = DateTime.Parse("16-04-2002"),
                ReceiveNewsLetters = false
            }

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
            foreach(PersonResponse personResponseFromAdd in personResponses)
            {
                if (personResponseFromAdd.PersonName != null)
                {
                    if (personResponseFromAdd.PersonName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                    {
                        Assert.Contains(personResponseFromAdd,personresponseListFiltered);
                    }
                }
            }


        }

        #endregion

        #region GetSortedPersons

        [Fact]
        public async Task GetSortedPersons()
        {
            //Arrange
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryAddRequest country_request_2 = new CountryAddRequest() { CountryName = "India" };

            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            CountryResponse country_response_2 = await _countriesServices.AddCountry(country_request_2);

            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = Gender.Male, Address = "address of smith", CountryID = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };

            PersonAddRequest person_request_2 = new PersonAddRequest() { PersonName = "Mary", Email = "mary@example.com", Gender = Gender.Female, Address = "address of mary", CountryID = country_response_2.CountryId, DateOfBirth = DateTime.Parse("2000-02-02"), ReceiveNewsLetters = false };

            PersonAddRequest person_request_3 = new PersonAddRequest() { PersonName = "Rahman", Email = "rahman@example.com", Gender = Gender.Male, Address = "address of rahman", CountryID = country_response_2.CountryId, DateOfBirth = DateTime.Parse("1999-03-03"), ReceiveNewsLetters = true };

            List<PersonAddRequest> person_requests = new List<PersonAddRequest>() { person_request_1, person_request_2, person_request_3 };

            List<PersonResponse> person_response_list_from_add = new List<PersonResponse>();

            foreach (PersonAddRequest person_request in person_requests)
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

            for(int i=0;i< person_response_list_from_add.Count; i++)
            {
                Assert.Equal(person_response_list_from_add[i], person_response_list_from_sort[i]);
            }
        }

        #endregion

        #region PersonUpdateRequest

        [Fact]
        public async Task PersonUpdateRequest_NullPersonUpdateRequest()
        {
            PersonUpdateRequest? personUpdateRequest = null;

            await Assert.ThrowsAsync<ArgumentNullException>(async() =>await _personsServices.UpdatePerson(personUpdateRequest));

        }

        [Fact]
        public async Task PersonUpdateRequest_InvalidPersonId()
        {
            PersonUpdateRequest? personUpdateRequest = new PersonUpdateRequest() { PersonID = Guid.NewGuid() };

            await Assert.ThrowsAsync<ArgumentException>(async() =>
            {
                await _personsServices.UpdatePerson(personUpdateRequest);
            });
        }

        [Fact]
        public async Task PersonUpdateRequest_NullPersonName()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = Gender.Male, Address = "address of smith", CountryID = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
            PersonResponse personResponse = await _personsServices.AddPerson(person_request_1);

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();
            personUpdateRequest.PersonName = null;

            await Assert.ThrowsAsync<ArgumentException>(async() => 
            {
                await _personsServices.UpdatePerson(personUpdateRequest);
            });


        }

        [Fact]
        public async Task PersonUpdateRequest_ProperDetails()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = Gender.Male, Address = "address of smith", CountryID = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
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

            Assert.Equal(person_response_from_get, person_response_from_update);


        }

        #endregion

        #region DeletePerson

        [Fact]
        public async Task DeletePersons_InvalidPersonId()
        {
            Guid newGuid = Guid.NewGuid();
            Assert.False(await _personsServices.DeletePerson(newGuid));
        }

        [Fact]
        public async Task DeletePersons_ProperPersonId()
        {
            CountryAddRequest country_request_1 = new CountryAddRequest() { CountryName = "USA" };
            CountryResponse country_response_1 = await _countriesServices.AddCountry(country_request_1);
            PersonAddRequest person_request_1 = new PersonAddRequest() { PersonName = "Smith", Email = "smith@example.com", Gender = Gender.Male, Address = "address of smith", CountryID = country_response_1.CountryId, DateOfBirth = DateTime.Parse("2002-05-06"), ReceiveNewsLetters = true };
            PersonResponse personResponse = await _personsServices.AddPerson(person_request_1);

            Assert.True(await _personsServices.DeletePerson(personResponse.PersonID));
        }

        #endregion
    }
}