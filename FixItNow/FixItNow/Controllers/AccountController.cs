using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Models.ViewModel;
using FixItNow.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FixItNow.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<MyApplicationUser> _signInManager;
        private readonly UserManager<MyApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly CurrentUserService _currentUserService;

        public AccountController(
            SignInManager<MyApplicationUser> signInManager,
            UserManager<MyApplicationUser> userManager,
            ApplicationDbContext db,
            CurrentUserService currentUserService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _db = db;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.ErrorMessage = "No account found with that email.";
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                ViewBag.ErrorMessage = "Invalid login attempt.";
                return View();
            }

            var appUser = _db.Userss.FirstOrDefault(u => u.email == email);
            if (appUser != null)
            {
                _currentUserService.SignIn(appUser.id, "user");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new MyApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                City = model.City,
                Gender = model.Gender
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            var appUser = _db.Userss.FirstOrDefault(u => u.email == model.Email);
            if (appUser == null)
            {
                appUser = new User
                {
                    firstName = model.FirstName,
                    lastName = model.LastName,
                    phoneNumber = model.PhoneNumber,
                    address = model.Address,
                    city = model.City,
                    gender = model.Gender,
                    email = model.Email,
                    password = string.Empty,
                    joinedTime = DateTime.Now
                };
                _db.Userss.Add(appUser);
            }

            if (!_db.Credentialss.Any(c => c.email == model.Email))
            {
                var credentials = new Credentials { email = model.Email, type = "user" };
                var passwordHasher = new PasswordHasher<Credentials>();
                credentials.password = passwordHasher.HashPassword(credentials, model.Password);
                _db.Credentialss.Add(credentials);
            }

            _db.SaveChanges();
            await _signInManager.SignInAsync(user, isPersistent: false);
            _currentUserService.SignIn(appUser.id, "user");

            return RedirectToAction("Index", "Home");
        }
    }
}
