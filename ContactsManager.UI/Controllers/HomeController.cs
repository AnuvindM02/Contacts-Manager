using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.Controllers
{
    public class HomeController : Controller
    {
        [Route("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            IExceptionHandlerPathFeature? exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null && exceptionHandlerPathFeature.Error != null)
            {
                ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            }
            return View();
        }
    }
}
