using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Models.Repository;
using FixItNow.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FixItNow.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;
        private readonly PasswordHasher<Credentials> _passwordHasher = new();

        public AuthenticationController(
            IWebHostEnvironment env,
            IConfiguration configuration,
            ApplicationDbContext d,
            CurrentUserService currentUserService)
        {
            _webHostEnvironment = env;
            _configuration = configuration;
            c = d;
            _currentUserService = currentUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string firstName, string lastName, string email, string phoneNumber, string address, string city, string gender, string pas)
        {
            AuthenticationRepository ar = new AuthenticationRepository(c);
            if (ar.isAlreadyExists(email))
            {
                ViewBag.Message = "Email is already associated with an account.";
                return View();
            }

            var user = new User
            {
                address = address,
                firstName = firstName,
                lastName = lastName,
                email = email,
                phoneNumber = phoneNumber,
                city = city,
                gender = gender,
                password = string.Empty,
                joinedTime = DateTime.Now
            };

            var credentials = new Credentials { email = email, type = "user" };
            credentials.password = _passwordHasher.HashPassword(credentials, pas);

            c.Userss.Add(user);
            c.Credentialss.Add(credentials);
            c.SaveChanges();

            return View("SignIn");
        }

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            var configuredAdminEmail = _configuration["Admin:Email"];
            var configuredAdminPassword = _configuration["Admin:Password"];
            var developmentAdminLogin = _webHostEnvironment.IsDevelopment() && email == "admin@fixitnow.com" && password == "adminfixitnow";
            var configuredAdminLogin = !string.IsNullOrWhiteSpace(configuredAdminEmail) &&
                                       !string.IsNullOrWhiteSpace(configuredAdminPassword) &&
                                       email == configuredAdminEmail &&
                                       password == configuredAdminPassword;

            if (developmentAdminLogin || configuredAdminLogin)
            {
                _currentUserService.SignIn(0, "admin");
                return RedirectToAction("Index", "Admin");
            }

            var credentials = c.Credentialss.FirstOrDefault(cred => cred.email == email);
            if (credentials == null || !IsPasswordValid(credentials, password))
            {
                ViewBag.x = "Incorrect email or password.";
                return View();
            }

            var id = -1;
            if (credentials.type == "user")
            {
                var user = c.Userss.FirstOrDefault(u => u.email == email);
                if (user == null)
                {
                    ViewBag.x = "Account profile was not found.";
                    return View();
                }

                id = user.id;
            }
            else if (credentials.type == "provider")
            {
                var provider = c.Providers.FirstOrDefault(p => p.Email == email);
                if (provider == null)
                {
                    ViewBag.x = "Provider profile was not found.";
                    return View();
                }

                if (!string.Equals(provider.status, "approved", StringComparison.OrdinalIgnoreCase))
                {
                    ViewBag.x = "Your provider account is pending approval.";
                    return View();
                }

                id = provider.Id;
            }

            if (id < 0)
            {
                ViewBag.x = "Account type is invalid.";
                return View();
            }

            _currentUserService.SignIn(id, credentials.type);
            return credentials.type == "provider"
                ? RedirectToAction("ProviderPanel", "Provider")
                : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProviderRegistration()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ProviderRegistration(string FirstName, string LastName, string Email, string Password, string RetypePassword, string Phone, string Address, string City, string Province, string CNIC, IFormFile Resume, IFormFile ProfilePicture, string Education, string skill, string about, string experience)
        {
            if (Password != RetypePassword)
            {
                ModelState.AddModelError("password", "Passwords do not match.");
                return View();
            }

            AuthenticationRepository ar = new AuthenticationRepository(c);
            if (ar.isAlreadyExists(Email))
            {
                ViewBag.Message = "Email is already associated with an account.";
                return View();
            }

            string wwwPath = _webHostEnvironment.WebRootPath;
            string uploadPath = Path.Combine(wwwPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            string? resumePath = null;
            string? profilePicPath = null;

            if (Resume != null)
            {
                if (!IsAllowedUpload(Resume, [".pdf", ".doc", ".docx"], 5 * 1024 * 1024))
                {
                    ModelState.AddModelError("Resume", "Upload a PDF/DOC/DOCX resume up to 5 MB.");
                    return View();
                }

                resumePath = SaveUpload(Resume, Path.Combine(wwwPath, "uploads", "resumes"), Path.Combine("uploads", "resumes"));
            }

            if (ProfilePicture != null)
            {
                if (!IsAllowedUpload(ProfilePicture, [".jpg", ".jpeg", ".png", ".webp"], 2 * 1024 * 1024))
                {
                    ModelState.AddModelError("ProfilePicture", "Upload a JPG/PNG/WEBP profile picture up to 2 MB.");
                    return View();
                }

                profilePicPath = SaveUpload(ProfilePicture, Path.Combine(wwwPath, "uploads", "profilePics"), Path.Combine("uploads", "profilePics"));
            }

            var provider = new Provider
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                Password = string.Empty,
                PhoneNumber = Phone,
                Address = Address,
                City = City,
                Province = Province,
                CNIC = CNIC,
                ResumePath = resumePath ?? string.Empty,
                ProfilePicPath = profilePicPath ?? string.Empty,
                Education = Education,
                status = "UnApproved",
                about_me = about,
                expereince = experience,
                skill = skill
            };

            var credentials = new Credentials { email = Email, type = "provider" };
            credentials.password = _passwordHasher.HashPassword(credentials, Password);

            c.Providers.Add(provider);
            c.Credentialss.Add(credentials);
            c.SaveChanges();

            return View("SignIn");
        }

        public IActionResult ProviderUI()
        {
            return View();
        }

        public IActionResult Logout()
        {
            _currentUserService.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool IsPasswordValid(Credentials credentials, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(credentials, credentials.password, password);
            if (result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                if (result == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    credentials.password = _passwordHasher.HashPassword(credentials, password);
                    c.SaveChanges();
                }

                return true;
            }

            // Upgrade existing plain-text development records after a successful login.
            if (credentials.password == password)
            {
                credentials.password = _passwordHasher.HashPassword(credentials, password);
                c.SaveChanges();
                return true;
            }

            return false;
        }

        private static string SaveUpload(IFormFile file, string fullDirectory, string relativeDirectory)
        {
            Directory.CreateDirectory(fullDirectory);
            var safeName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName).ToLowerInvariant()}";
            var fullPath = Path.Combine(fullDirectory, safeName);
            using var stream = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(stream);

            return Path.Combine(relativeDirectory, safeName).Replace("\\", "/");
        }

        private static bool IsAllowedUpload(IFormFile file, string[] allowedExtensions, long maxBytes)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            return file.Length > 0 && file.Length <= maxBytes && allowedExtensions.Contains(extension);
        }
    }
}
