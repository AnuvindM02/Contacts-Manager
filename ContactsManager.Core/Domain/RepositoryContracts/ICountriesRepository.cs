using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace RepositoryContracts
{
    public interface ICountriesRepository
    {
        Country AddCountry(Country country);

        Task<List<Country>> GetAllCountries();

        Task<Country?> GetCountryById(Guid countryID);

        Country? GetCountryByName(string countryName);
    }
}
