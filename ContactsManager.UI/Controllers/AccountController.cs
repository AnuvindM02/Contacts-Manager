using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using CRUDXunitTest.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    [AllowAnonymous]//This controller be accessed without authorization
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if(ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors)
                    .Select(temp => temp.ErrorMessage);
                return View(registerDTO);
            }

            ApplicationUser user  = new ApplicationUser()
            {
                Email=registerDTO.Email,PhoneNumber=registerDTO.Phone,UserName = registerDTO.Email,
                PersonName = registerDTO.PersonName
            };

            IdentityResult result = await _userManager.CreateAsync(user,registerDTO.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(PersonController.Index), "Person");
            }
            else
            {
                foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("Register", error.Description);
                }
                return View(registerDTO);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO,string? ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors)
                    .Select(temp => temp.ErrorMessage);
                return View(loginDTO);
            }
            var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password,isPersistent:false,lockoutOnFailure:false);

            if (result.Succeeded)
            {
                if(!string.IsNullOrEmpty(ReturnUrl)&&Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }
                return RedirectToAction(nameof(PersonController.Index), "Person");
            }
            else
            {
                ModelState.AddModelError("Login", "Invalid email or password");
            }
            return View(loginDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json(false);
            }
        }
    }
}
