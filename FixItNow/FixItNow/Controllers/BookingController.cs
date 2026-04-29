using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;

        public BookingController(ApplicationDbContext d, CurrentUserService currentUserService)
        {
            c = d;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "user")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var serviceExists = await c.Services.AnyAsync(s => s.id == id);
            if (!serviceExists)
            {
                return NotFound();
            }

            TempData["ServiceId"] = id;
            ViewBag.x = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string taskDetails, string address, string pricing, DateTime serviceDate, DateTime serviceTime)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "user")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            if (!int.TryParse(Convert.ToString(TempData["ServiceId"]), out var id))
            {
                return RedirectToAction("Index", "Services");
            }

            var service = await c.Services.FirstOrDefaultAsync(s => s.id == id);
            if (service == null)
            {
                return NotFound();
            }

            if (serviceDate.Date < DateTime.Now.Date)
            {
                ViewBag.DateError = "Select a future date.";
                ViewBag.x = id;
                TempData["ServiceId"] = id;
                return View("Index");
            }

            if (currentUser.Id == service.providerID)
            {
                ViewBag.Error = "You cannot book your own service.";
                ViewBag.x = id;
                TempData["ServiceId"] = id;
                return View("Index");
            }

            var booking = new Booking
            {
                providerId = service.providerID,
                serviceId = service.id,
                description = taskDetails,
                address = address,
                pricing = pricing,
                Time = serviceTime,
                Date = serviceDate,
                customerId = currentUser.Id
            };

            c.Bookings.Add(booking);
            await c.SaveChangesAsync();

            ViewBag.Confirmation = "Booking confirmed.";
            return View();
        }
    }
}
