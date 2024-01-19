 using System;
using System.Collections.Generic;
using Entities;
namespace ServiceContracts.DTO
{
    public class CountryResponse
    {
        public Guid CountryId { get; set; }
        public string? CountryName { get; set; }

        public override bool Equals(object? obj)
        {
            if(obj == null)
                return false;
            if (obj.GetType() != typeof(CountryResponse))
                return false;

            CountryResponse country_to_compare = (CountryResponse)obj;

            return this.CountryId == country_to_compare.CountryId && this.CountryName == country_to_compare.CountryName;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public static class CountryExtensions
    {
        public static CountryResponse ToCountryResponse(this Country country)
        {
            return new CountryResponse { CountryId = country.CountryID, CountryName = country.CountryName };
        }
    }
}