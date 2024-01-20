using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using CRUDXunitTest.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> usermanager, SignInManager<ApplicationUser> signInManager)
        {
            _usermanager = usermanager;
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

            IdentityResult result = await _usermanager.CreateAsync(user,registerDTO.Password);

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
        public async Task<IActionResult> Login(LoginDTO loginDTO)
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
    }
}
