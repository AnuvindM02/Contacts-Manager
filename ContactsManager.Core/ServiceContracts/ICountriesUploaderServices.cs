using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface ICountriesUploaderServices
    {
        int UploadCountriesFromExcelFile(IFormFile formFile);
    }
}