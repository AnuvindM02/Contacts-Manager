using Entities.Enums;
using ServiceContracts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts
{
    public interface IPersonsDeleterServices
    {

        Task<bool> DeletePerson(Guid? PersonId);
     
    }
}
