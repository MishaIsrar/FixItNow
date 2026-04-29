using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Models.Repository;
using FixItNow.Models.ViewModel;
using FixItNow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;

        public AdminController(ApplicationDbContext d, CurrentUserService currentUserService)
        {
            c = d;
            _currentUserService = currentUserService;
        }

        public async Task<IActionResult> Index()
        {
            if (!await IsAdminAsync())
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            AdminRepository repository = new AdminRepository(c);
            Admin admin = new Admin
            {
                users = repository.getListOfUsers(),
                providers = repository.getListOfProviders(),
                reviews = repository.getListOfReviews(),
                bookings = repository.getListOfBooking()
            };

            admin.revenue = admin.bookings.Sum(item => decimal.TryParse(item.pricing, out var value) ? value : 0);
            return View(admin);
        }

        public async Task<IActionResult> Approve(int id)
        {
            if (!await IsAdminAsync())
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var provider = await c.Providers.FirstOrDefaultAsync(p => p.Id == id);
            if (provider == null)
            {
                return NotFound();
            }

            provider.status = "approved";
            await c.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Reject(int id)
        {
            if (!await IsAdminAsync())
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var provider = await c.Providers.FirstOrDefaultAsync(p => p.Id == id);
            if (provider == null)
            {
                return NotFound();
            }

            provider.status = "rejected";
            await c.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AllUsers()
        {
            if (!await IsAdminAsync())
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            AdminRepository repository = new AdminRepository(c);
            List<User> users = repository.getListOfUsers();
            return View(users);
        }

        public async Task<IActionResult> AllBookings()
        {
            if (!await IsAdminAsync())
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            AdminRepository repository = new AdminRepository(c);
            List<Booking> bookings = repository.getListOfBooking();
            return View(bookings);
        }

        private async Task<bool> IsAdminAsync()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            return currentUser?.Type == "admin";
        }
    }
}
