using ServiceContracts.DTO;
using Microsoft.AspNetCore.Http;

namespace ServiceContracts
{
    public interface ICountriesUploaderServices
    {
        Task<int> UploadCountriesFromExcelFile(IFormFile formFile);
    }
}