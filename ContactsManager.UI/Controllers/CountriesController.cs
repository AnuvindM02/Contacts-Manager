using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using Services;

namespace CRUDXunitTest.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesUploaderServices _countriesUploaderServices;

        public CountriesController(ICountriesUploaderServices countriesUploaderServices)
        {
            _countriesUploaderServices = countriesUploaderServices;
        }

        [Route("[action]")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult UploadFromExcel(IFormFile excelFile)
        {
            if(excelFile == null || excelFile.Length==0)
            {
                ViewBag.Message = "No file provided!";
                return View();
            }
            else if(!Path.GetExtension(excelFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Message = "Unsupported file type! Please select an xlsx file";
                return View();
            }

            int countriesAdded = _countriesUploaderServices.UploadCountriesFromExcelFile(excelFile);

            ViewBag.Message = $"{countriesAdded} countries added";
            return View();
        }
    }
}
