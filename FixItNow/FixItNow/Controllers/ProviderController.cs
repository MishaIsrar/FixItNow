using FixItNow.Data;
using FixItNow.Models.Repository;
using FixItNow.Models.ViewModel;
using FixItNow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class ProviderController : Controller
    {
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;

        public ProviderController(ApplicationDbContext d, CurrentUserService currentUserService)
        {
            c = d;
            _currentUserService = currentUserService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var provider = await c.Providers.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (provider == null)
            {
                return NotFound();
            }

            ProviderRepository repository = new ProviderRepository(c);
            ProviderProfile profile = new ProviderProfile
            {
                provider = provider,
                projects = repository.getTotalProjects(id),
                clients = repository.getTotalClients(id),
                positiveReviews = repository.getPositiveReviews(id),
                clientSatisfaction = repository.getSatisfactionRate(id)
            };

            return View(profile);
        }

        public async Task<IActionResult> ProviderPanel()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser?.Type != "provider")
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            int id = currentUser.Id;
            ProviderPanel panel = new ProviderPanel();
            var bookings = await c.Bookings.AsNoTracking().Where(b => b.providerId == id).ToListAsync();
            var services = await c.Services.AsNoTracking().Where(s => s.providerID == id).ToListAsync();
            var messages = await c.Messagess.AsNoTracking().Where(m => m.providerId == id).ToListAsync();

            var titles = bookings
                .Join(services, booking => booking.serviceId, service => service.id, (booking, service) => service.title)
                .ToList();

            panel.countOfBookings = services
                .Select(s => bookings.Count(b => b.serviceId == s.id))
                .ToList();

            panel.revenueOfServices = services
                .Select(s => bookings
                    .Where(b => b.serviceId == s.id)
                    .Sum(b => int.TryParse(b.pricing, out var value) ? value : 0))
                .ToList();

            panel.services = services;
            panel.titles = titles;
            panel.messages = messages;
            panel.bookings = bookings;

            return View(panel);
        }
    }
}
