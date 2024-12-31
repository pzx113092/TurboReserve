using System.Drawing.Printing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve.Models;
using TurboReserve.Models.Enums;
using TurboReserve.Services;

namespace TurboReserve.Controllers
{
    [Authorize(Roles = "ServiceProvider")]
    public class ServiceProviderPanelController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ServiceProviderPanelController(
            IServiceService serviceService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _serviceService = serviceService;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "ServiceProvider")]
        public async Task<IActionResult> Schedule(DateTime? from, DateTime? to)
        {
            var userId = _userManager.GetUserId(User);

            
            var startDate = from ?? DateTime.Today;
            var endDate = to ?? DateTime.Today.AddDays(7);

            
            if (startDate > endDate)
            {
                TempData["ErrorMessage"] = "Data początkowa nie może być późniejsza niż data końcowa.";
                return RedirectToAction(nameof(Schedule));
            }

            
            var scheduleSlots = await _context.ScheduleSlots
                .Include(slot => slot.Service)
                .Where(slot => slot.Service.ServiceProvider.IdentityUserId == userId &&
                               slot.StartTime.Date >= startDate && slot.StartTime.Date <= endDate)
                .ToListAsync();

            
            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

            return View(scheduleSlots);
        }


        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            int serviceProviderId = await GetServiceProviderIdByUserId(userId);

            var services = await _serviceService.GetByServiceProviderIdAsync(serviceProviderId);
            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service model)
        {
            var userId = _userManager.GetUserId(User);
            var serviceProviderId = await GetServiceProviderIdByUserId(userId);
            model.ServiceProviderId = serviceProviderId; 
            var existingServiceCount = await _context.Services
                .Where(s => s.ServiceProviderId == serviceProviderId)
                .CountAsync();

            if (existingServiceCount >= 20)
            {
                TempData["ErrorMessage"] = "Nie możesz dodać więcej niż 20 usług.";
                return RedirectToAction(nameof(Index));
            }


            TempData["SuccessMessage"] = "Dodano usługę";
            await _serviceService.AddAsync(model);
            return RedirectToAction(nameof(Index));
        }
        

        [Authorize(Roles = "ServiceProvider")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateService(ServiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);

            
            var serviceCount = await _context.Services
                .Where(s => s.ServiceProvider.IdentityUserId == userId)
                .CountAsync();

            if (serviceCount >= 20)
            {
                TempData["ErrorMessage"] = "Nie możesz dodać więcej niż 20 usług.";
                return RedirectToAction("Index");
            }

            var serviceProvider = await _context.ServiceProviders
                .FirstOrDefaultAsync(sp => sp.IdentityUserId == userId);

            if (serviceProvider == null)
            {
                TempData["ErrorMessage"] = "Nie znaleziono usługodawcy.";
                return RedirectToAction("Index");
            }

            var newService = new Service
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ServiceProviderId = serviceProvider.Id
            };

            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Usługa została dodana pomyślnie.";
            return RedirectToAction("Index");
        }



        [Authorize(Roles = "ServiceProvider")]
        public async Task<IActionResult> EditService(int id)
        {
            var userId = _userManager.GetUserId(User);

            var service = await _context.Services
                .Where(s => s.ServiceProvider.IdentityUserId == userId && s.Id == id)
                .FirstOrDefaultAsync();

            if (service == null)
            {
                TempData["ErrorMessage"] = "Nie znaleziono usługi lub nie masz do niej dostępu.";
                return RedirectToAction("Index");
            }

            var model = new Service
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                Price = service.Price
            };
            return View(model);
        }

        [Authorize(Roles = "ServiceProvider")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditService(Service model)
        {
            var userId = _userManager.GetUserId(User);
            var service = await _context.Services
                .Where(s => s.ServiceProvider.IdentityUserId == userId && s.Id == model.Id)
                .FirstOrDefaultAsync();

            if (service == null)
            {
                TempData["ErrorMessage"] = "Nie znaleziono usługi lub nie masz do niej dostępu.";
                return RedirectToAction("Index");
            }

            service.Name = model.Name;
            service.Description = model.Description;
            service.Price = model.Price;
            _context.Update(service);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Usługa została zaktualizowana pomyślnie.";
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var serviceProviderId = await GetServiceProviderIdByUserId(userId);
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null || service.ServiceProviderId != serviceProviderId)
            {
                return NotFound();
            }
            return View(service);
        }

        public async Task<IActionResult> ManageReservations(DateTime? from, DateTime? to)
        {
            var userId = _userManager.GetUserId(User);
            var startDate = from ?? DateTime.Today;
            var endDate = to ?? DateTime.Today.AddDays(7);

            var reservations = await _context.Reservations
                .Include(r => r.Service)
                .ThenInclude(s => s.ServiceProvider)
                .Where(r => r.Service.ServiceProvider.IdentityUserId == userId &&
                            r.StartTime.Date >= startDate && r.StartTime.Date <= endDate)
                .Select(r => new ReservationViewModel
                {
                    ReservationId = r.Id,
                    ServiceName = r.Service.Name,
                    CustomerName = r.Customer.UserName,
                    ReservationDate = r.ReservationDate,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Status = r.Status.ToString()
                })
                .ToListAsync();

            ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

            return View(reservations);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateReservationStatus(int id, string status)
        {
            var userId = _userManager.GetUserId(User);

            var reservation = await _context.Reservations
                .Include(r => r.Service)
                .ThenInclude(s => s.ServiceProvider)
                .FirstOrDefaultAsync(r => r.Id == id && r.Service.ServiceProvider.IdentityUserId == userId);

            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Nie znaleziono rezerwacji lub nie masz do niej dostępu.";
                return RedirectToAction("ManageReservations");
            }

            if (!Enum.TryParse<ReservationStatus>(status, true, out var newStatus))
            {
                TempData["ErrorMessage"] = "Nieprawidłowy status.";
                return RedirectToAction("ManageReservations");
            }

            reservation.Status = newStatus;

            _context.Update(reservation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Status rezerwacji został zaktualizowany.";
            return RedirectToAction("ManageReservations");
        }


        private async Task<int> GetServiceProviderIdByUserId(string userId)
        {
            var serviceProvider = await _context.ServiceProviders
                .FirstOrDefaultAsync(sp => sp.IdentityUserId == userId);

            if (serviceProvider == null)
            {
                throw new InvalidOperationException("Nie znaleziono usługodawcy dla zalogowanego użytkownika.");
            }
            return serviceProvider.Id;
        }
    }
}
