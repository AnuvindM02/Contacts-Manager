using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using CRUDXunitTest.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactsManager.UI.Controllers
{
    [Route("[controller]/[action]")]
/*    [AllowAnonymous]//This controller be accessed without authorization
*/    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        [HttpGet]
        [Authorize("NotAuthorized")]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [Authorize("NotAuthorized")]
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
                //Check user type
                if(registerDTO.UserType == UserTypeOptions.Admin)
                {
                    //Add Admin Role
                    if(await _roleManager.FindByNameAsync(UserTypeOptions.Admin.ToString()) is null)
                    {
                        ApplicationRole applicationRole = new ApplicationRole()
                        {
                            Name = UserTypeOptions.Admin.ToString()
                        };
                        await _roleManager.CreateAsync(applicationRole);
                    }

                    //Add user into admin role
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.Admin.ToString());

                }
                else
                {
                    //Add user into default role
                    await _userManager.AddToRoleAsync(user, UserTypeOptions.User.ToString());
                }

                //Sign in
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
        [Authorize("NotAuthorized")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Authorize("NotAuthorized")]
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
                //If the user is admin
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, UserTypeOptions.Admin.ToString()))
                    {
                        return RedirectToAction("Index","Home",new {area="Admin"});
                    }
                }

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

        [Authorize]
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
