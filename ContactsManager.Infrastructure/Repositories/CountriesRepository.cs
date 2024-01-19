using Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _db;
        public CountriesRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public Country AddCountry(Country country)
        {
            _db.Countries.Add(country);
            _db.SaveChanges();
            return country;
        }

        public async Task<List<Country>> GetAllCountries()
        {
            return await _db.Countries.ToListAsync();
        }

        public async Task<Country?> GetCountryById(Guid countryID)
        {
            return await _db.Countries.FirstOrDefaultAsync(temp => temp.CountryID == countryID);
        }

        public Country? GetCountryByName(string countryName)
        {
            return _db.Countries.FirstOrDefault(temp => temp.CountryName == countryName);
        }
    }
}
