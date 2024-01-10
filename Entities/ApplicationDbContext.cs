using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    public class ApplicationDbContext:DbContext
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        public ApplicationDbContext(DbContextOptions options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seeding countries data
            string countriesJson = System.IO.File.ReadAllText("countries.json");
            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);

            foreach (Country country in countries)
                modelBuilder.Entity<Country>().HasData(country);

            //Seeding persons data
            string personsJson = System.IO.File.ReadAllText("persons.json");
            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);

            foreach (Person person in persons)
                modelBuilder.Entity<Person>().HasData(person);


            //Fluent API
            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)");

            modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();

            modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public Task<int> sp_InsertPersonAsync(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),                
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };
            return Database.ExecuteSqlRawAsync("EXECUTE [dbo].[InsertPerson] @PersonID,@PersonName,@Email,@Address,@CountryID,@Gender,@DateOfBirth,@ReceiveNewsLetters", parameters);
        }

        public Task<int> sp_DeletePersonAsync(Person person)
        {
            SqlParameter parameter = new SqlParameter("@PersonID", person.PersonID);
            return Database.ExecuteSqlRawAsync("EXECUTE [dbo].[DeletePerson] @PersonID", parameter);
        }

        public Task<int> sp_UpdatePersonAsync(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PersonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName),
                new SqlParameter("@Email", person.Email),
                new SqlParameter("@Address", person.Address),
                new SqlParameter("@CountryID", person.CountryID),
                new SqlParameter("@Gender", person.Gender),
                new SqlParameter("@DateOfBirth", person.DateOfBirth),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)
            };
            return Database.ExecuteSqlRawAsync("EXECUTE [dbo].[UpdatePerson] @PersonID,@PersonName,@Email,@Address,@CountryID,@Gender,@DateOfBirth,@ReceiveNewsLetters", parameters);
        }
    }

}
