using FixItNow.Data;
using FixItNow.Models;
using FixItNow.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FixItNow.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext c;
        private readonly CurrentUserService _currentUserService;

        public MessageController(ApplicationDbContext d, CurrentUserService currentUserService)
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

            if (!await c.Services.AnyAsync(s => s.id == id))
            {
                return NotFound();
            }

            TempData["ServiceId"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string from, string messagetitle, string message)
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

            if (currentUser.Id == service.providerID)
            {
                ViewBag.Error = "Cannot send message to yourself.";
                TempData["ServiceId"] = id;
                return View();
            }

            var userMessage = new Messages
            {
                title = messagetitle,
                message = message,
                senderName = from,
                serviceName = service.title,
                DateTime = DateTime.Now.ToString("g"),
                serviceId = service.id,
                providerId = service.providerID,
                customerId = currentUser.Id
            };

            c.Messagess.Add(userMessage);
            await c.SaveChangesAsync();

            ViewBag.Sent = "Message sent.";
            return View();
        }

        public async Task<IActionResult> Inbox()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser == null || (currentUser.Type != "user" && currentUser.Type != "provider"))
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var list = currentUser.Type == "user"
                ? await c.Messagess.AsNoTracking().Where(m => m.customerId == currentUser.Id).ToListAsync()
                : await c.Messagess.AsNoTracking().Where(m => m.providerId == currentUser.Id).ToListAsync();

            return View(list);
        }

        public async Task<IActionResult> Read(int id)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("SignIn", "Authentication");
            }

            var message = await c.Messagess.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            var canRead = (currentUser.Type == "user" && message.customerId == currentUser.Id) ||
                          (currentUser.Type == "provider" && message.providerId == currentUser.Id) ||
                          currentUser.Type == "admin";

            return canRead ? View(message) : Forbid();
        }
    }
}
