using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Models.Repository;
using FixItNow.Models.ViewModel;
using FixItNow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class ServicesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;

        public ServicesController(IWebHostEnvironment env, ApplicationDbContext d, CurrentUserService currentUserService)
        {
            _webHostEnvironment = env;
            c = d;
            _currentUserService = currentUserService;
        }

        public IActionResult MyAction(string inputData)
        {
            return PartialView("_MyPartial", model: inputData);
        }

        public async Task<IActionResult> Index()
        {
            var service = await c.Services.AsNoTracking().ToListAsync();
            return View(service);
        }

        [HttpGet]
        public async Task<IActionResult> AddService()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            return currentUser?.Type == "provider" ? View() : RedirectToAction("SignIn", "Authentication");
        }

        [HttpPost]
        public async Task<IActionResult> AddService(IFormFile serviceImage, string provider, string title, string description, string category, string features, float price)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "provider")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || string.IsNullOrWhiteSpace(category) || price <= 0)
            {
                ViewBag.ErrorMessage = "Title, description, category, and a valid price are required.";
                return View();
            }

            string? imagePath = null;
            if (serviceImage != null)
            {
                if (!IsAllowedUpload(serviceImage, [".jpg", ".jpeg", ".png", ".webp"], 3 * 1024 * 1024))
                {
                    ViewBag.ErrorMessage = "Upload a JPG/PNG/WEBP image up to 3 MB.";
                    return View();
                }

                imagePath = SaveUpload(serviceImage, Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "UploadedImages"), Path.Combine("uploads", "UploadedImages"));
            }

            var providerProfile = await c.Providers.FirstOrDefaultAsync(p => p.Id == currentUser.Id);
            var featureList = (features ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var service = new Service
            {
                title = title.Trim(),
                description = description.Trim(),
                category = category.Trim(),
                features = featureList,
                provider = string.IsNullOrWhiteSpace(provider) ? $"{providerProfile?.FirstName} {providerProfile?.LastName}".Trim() : provider.Trim(),
                pricing = price,
                providerID = currentUser.Id,
                referenceImagePath = imagePath ?? "images/image.png"
            };

            c.Services.Add(service);
            await c.SaveChangesAsync();

            return RedirectToAction("ProviderPanel", "Provider");
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetails(int id)
        {
            var service = await c.Services.AsNoTracking().FirstOrDefaultAsync(s => s.id == id);
            if (service == null)
            {
                return NotFound();
            }

            var reviews = await c.Reviews.AsNoTracking().Where(r => r.serviceId == id).ToListAsync();
            var relatedServices = await c.Services.AsNoTracking()
                .Where(y => y.category == service.category && y.id != id)
                .Take(6)
                .ToListAsync();

            var model = new ServicesDetails
            {
                service = service,
                Reviews = reviews,
                oneStar = reviews.Count(r => r.rating == 1),
                twoStar = reviews.Count(r => r.rating == 2),
                threeStar = reviews.Count(r => r.rating == 3),
                fourStar = reviews.Count(r => r.rating == 4),
                fiveStar = reviews.Count(r => r.rating >= 5),
                totalReviews = reviews.Count,
                RelatedServices = relatedServices
            };

            return View(model);
        }

        public async Task<IActionResult> Save(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "user")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var service = await c.Services.FirstOrDefaultAsync(s => s.id == id);
            if (service == null)
            {
                return NotFound();
            }

            var alreadySaved = await c.SavedServicess.AnyAsync(s => s.serviceId == id && s.customerId == currentUser.Id);
            if (!alreadySaved)
            {
                c.SavedServicess.Add(new SavedServices
                {
                    serviceId = id,
                    serviceName = service.title,
                    customerId = currentUser.Id
                });
                await c.SaveChangesAsync();
            }

            return RedirectToAction("ServiceDetails", "Services", new { id });
        }

        public async Task<IActionResult> FilteredView(int id)
        {
            var filteredContent = id switch
            {
                1 => "Construction",
                2 => "Content Creation",
                3 => "Technology & IT Services",
                4 => "Designing and Creativity",
                5 => "Animation & Video Editing",
                6 => "Education & Training",
                7 => "Business & Administrative Services",
                8 => "Healthcare & Wellness",
                9 => "Legal & Financial Services",
                10 => "Home & Lifestyle Services",
                11 => "Repair & Maintenance",
                12 => "Logistics & Delivery",
                13 => "Entertainment & Leisure",
                14 => "Freelance & Gig Services",
                15 => "Digital Marketing",
                _ => string.Empty
            };

            var service = string.IsNullOrEmpty(filteredContent)
                ? new List<Service>()
                : await c.Services.AsNoTracking().Where(s => s.category == filteredContent).ToListAsync();

            return View(service);
        }

        public async Task<IActionResult> UserCart()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "user")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var bookings = await c.Bookings.AsNoTracking().Where(b => b.customerId == currentUser.Id).ToListAsync();
            var savedServices = await c.SavedServicess.AsNoTracking().Where(s => s.customerId == currentUser.Id).ToListAsync();
            var repository = new ServicesRepository(c);

            return View(new UserCart
            {
                Bookings = bookings,
                SavedServices = savedServices,
                exists = repository.getListOfExist(currentUser.Id, bookings)
            });
        }

        public async Task<IActionResult> RemoveFromSaved(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "user")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var savedService = await c.SavedServicess.FirstOrDefaultAsync(ss => ss.id == id && ss.customerId == currentUser.Id);
            if (savedService == null)
            {
                return NotFound();
            }

            c.SavedServicess.Remove(savedService);
            await c.SaveChangesAsync();
            return RedirectToAction("UserCart");
        }

        public async Task<IActionResult> RemoveService(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser == null || (currentUser.Type != "provider" && currentUser.Type != "admin"))
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var service = await c.Services.FirstOrDefaultAsync(s => s.id == id);
            if (service == null)
            {
                return NotFound();
            }

            if (currentUser.Type == "provider" && service.providerID != currentUser.Id)
            {
                return Forbid();
            }

            c.Services.Remove(service);
            await c.SaveChangesAsync();
            return RedirectToAction("ProviderPanel", "Provider");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            var service = await c.Services.FirstOrDefaultAsync(b => b.id == id);
            if (service == null)
            {
                return NotFound();
            }

            if (currentUser?.Type != "provider" || service.providerID != currentUser.Id)
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            TempData["ServiceId"] = id;
            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string Title, string Description, float Price)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "provider")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            int id = Convert.ToInt32(TempData["ServiceId"]);
            var service = await c.Services.FirstOrDefaultAsync(s => s.id == id && s.providerID == currentUser.Id);
            if (service == null)
            {
                return NotFound();
            }

            service.title = Title;
            service.description = Description;
            service.pricing = Price;
            await c.SaveChangesAsync();

            return RedirectToAction("ProviderPanel", "Provider");
        }

        public async Task<IActionResult> SortByPricing()
        {
            var service = await c.Services.AsNoTracking().OrderBy(s => s.pricing).ToListAsync();
            return View(service);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return PartialView("_serviceItem", new List<Service>());
            }

            var services = await c.Services.AsNoTracking()
                .Where(s => s.title.Contains(title))
                .Take(20)
                .ToListAsync();

            return PartialView("_serviceItem", services);
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
