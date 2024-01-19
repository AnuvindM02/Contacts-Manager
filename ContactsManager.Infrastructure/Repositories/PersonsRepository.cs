using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System.Linq.Expressions;

namespace Repositories
{
    public class PersonsRepository : IPersonsRepository
    {
        private readonly ApplicationDbContext _db;
        public PersonsRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Person> AddPerson(Person person)
        {
            _db.Persons.Add(person);
            await _db.SaveChangesAsync();
            return person;
        }

        public async Task<bool> DeletePersonById(Guid PersonID)
        {
            _db.Persons.RemoveRange(_db.Persons.Where(temp => temp.PersonID == PersonID));
            int rowsDeleted = await _db.SaveChangesAsync();

            return rowsDeleted > 0;

        }

        public async Task<List<Person>> GetAllPersons()
        {
            return await _db.Persons.Include("Country").ToListAsync();
        }

        public async Task<List<Person>> GetFilteredPersons(Expression<Func<Person, bool>> predicate)
        {
            return await _db.Persons.Include("Country")
                .Where(predicate).ToListAsync();
        }

        public async Task<Person?> GetPersonByPersonId(Guid personId)
        {
            return await _db.Persons.Include("Country")
                .FirstOrDefaultAsync(temp => temp.PersonID==personId);
        }

        public async Task<Person> UpdatePerson(Person person)
        {
            // Load Person with related Country entity
            Person? matchingPerson = await _db.Persons
                .Include("Country")  // Assuming "Country" is the navigation property
                .FirstOrDefaultAsync(temp => temp.PersonID == person.PersonID);

            if (matchingPerson == null)
            {
                // Handle the case where the person to update is not found
                return null;
            }

            // Update Person properties
            matchingPerson.PersonName = person.PersonName;
            matchingPerson.Email = person.Email;
            matchingPerson.DateOfBirth = person.DateOfBirth;
            matchingPerson.Gender = person.Gender;
            matchingPerson.CountryID = person.CountryID;
            matchingPerson.Address = person.Address;
            matchingPerson.ReceiveNewsLetters = person.ReceiveNewsLetters;

            // Save changes to the database
            int countUpdated = await _db.SaveChangesAsync();

            return matchingPerson;
        }
        
    }
}
