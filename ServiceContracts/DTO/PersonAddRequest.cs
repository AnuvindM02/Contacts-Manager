using Entities.Enums;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage ="Person Name is needed")]
        public string? PersonName { get; set; }

        [Required(ErrorMessage = "Email can't be blank")]
        [EmailAddress(ErrorMessage = "Email value should be a valid email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public Guid? CountryID { get; set; }
        [Required(ErrorMessage ="Gender must be selected")]
        public Gender? Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public bool ReceiveNewsLetters { get; set; }


        public Person ToPerson()
        {
            return new Person() { PersonName = PersonName, Gender = Gender.ToString(), Address = Address, CountryID = CountryID, Email = Email,DateOfBirth = DateOfBirth, ReceiveNewsLetters = ReceiveNewsLetters};
        }

    }
}
