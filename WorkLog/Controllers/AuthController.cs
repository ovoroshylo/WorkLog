using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WorkLog.Data;
using WorkLog.Models;

namespace WorkLog.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View("SignIn");
        }

        public IActionResult SignIn()
        {
            return View("SignIn");
        }

        /*
            Simple SignIn API 
        */
        [HttpPost]
        /*
            Check forgery token 
        */
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInVM signIn, string ReturnUrl = "/Channel/AddChannel")
        {
            IdentityUser user;

            if (ModelState.IsValid)
            {
                user = await _userManager.FindByEmailAsync(signIn.Email);

                if (user == null)
                {
                    ModelState.AddModelError("", "Email does not exist");
                    return View(signIn);
                }
                var result = await _signInManager.PasswordSignInAsync(user, signIn.Password, signIn.RememberMe, true);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Password is not correct");
                    return View(signIn);
                }
                return LocalRedirect(ReturnUrl);
            }
            return View(signIn);
        }

        public IActionResult Register()
        {
            return View("Register");
        }
        
        /*
            Simple Register API 
        */
        [HttpPost]
        /*
            Check forgery token 
        */
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View(register);
            IdentityUser newUser = new IdentityUser
            {
                Email = register.Email,
                UserName = register.Username
            };

            IdentityResult result = await _userManager.CreateAsync(newUser, register.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(register);
            }
            
            /*
                Set role as User as default.
            */
            result = await _userManager.AddToRoleAsync(newUser, "User");
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View(register);
            }
            return RedirectToAction("SignIn");
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        /*
            Add new Role(just for adding role to database) 
        */
        [HttpGet]
        public async Task<string> AddRole(string role)
        {
            IdentityRole newRole = new IdentityRole
            {
                Name = role
            };
            IdentityResult result = await _roleManager.CreateAsync(newRole);
            if (result.Succeeded)
                return "Success";
            else
                return "Failed";
        }

    }
}
