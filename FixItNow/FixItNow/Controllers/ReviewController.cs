using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;

        public ReviewController(ApplicationDbContext d, CurrentUserService currentUserService)
        {
            c = d;
            _currentUserService = currentUserService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(int rating, int serviceid, string comments)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "user")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            if (rating < 1 || rating > 5)
            {
                rating = Math.Clamp(rating, 1, 5);
            }

            var user = await c.Userss.FirstOrDefaultAsync(u => u.id == currentUser.Id);
            var serviceExists = await c.Services.AnyAsync(s => s.id == serviceid);
            if (user == null || !serviceExists)
            {
                return NotFound();
            }

            var alreadyReviewed = await c.Reviews.AnyAsync(r => r.customerId == currentUser.Id && r.serviceId == serviceid);
            if (!alreadyReviewed)
            {
                Review review = new Review
                {
                    comments = comments,
                    dateTime = DateTime.Now,
                    rating = rating,
                    serviceId = serviceid,
                    customerId = currentUser.Id,
                    name = user.firstName
                };

                c.Reviews.Add(review);
                await c.SaveChangesAsync();
            }

            return RedirectToAction("UserCart", "Services");
        }
    }
}
