using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Enums;
namespace Entities
{
    public class Person
    {
        [StringLength(40)]
        public string? PersonName { get; set; }

        [StringLength(40)]
        public string? Email {  get; set; }

        [StringLength(200)]
        public string? Address { get; set;}

        [Key]
        public Guid? PersonID { get; set; }
        public Guid? CountryID { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool ReceiveNewsLetters { get; set; }

        public string? TIN {  get; set; }

        [ForeignKey(nameof(CountryID))]
        public virtual Country? Country { get; set; }

    }
}
