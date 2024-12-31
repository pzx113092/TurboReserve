using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TurboReserve.Data;
using TurboReserve.Models;
using TurboReserve.Models.Enums;


namespace TurboReserve.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomerController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> ServiceSchedule(int id)
        {
            var service = await _context.Services
                .Include(s => s.ScheduleSlots.Where(slot => !slot.IsBooked))
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            var viewModel = new ServiceScheduleViewModel
            {
                ServiceId = service.Id,
                ServiceName = service.Name,
                ScheduleSlots = service.ScheduleSlots.Select(slot => new ScheduleSlotViewModel
                {
                    SlotId = slot.Id,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime
                }).ToList()
            };

            return View(viewModel);
        }



        public async Task<IActionResult> ServiceProviderDetails(int id)
        {
            var provider = await _context.ServiceProviders
                .Include(sp => sp.Services)
                .FirstOrDefaultAsync(sp => sp.Id == id);

            if (provider == null)
            {
                return NotFound();
            }

            var viewModel = new ServiceProviderViewModel
            {
                ServiceProviderId = provider.Id,
                BusinessName = provider.BusinessName,
                Address = provider.Address,
                City = provider.City,
                ZipCode = provider.ZipCode,
                Latitude = provider.Latitude,
                Longitude = provider.Longitude,
                Services = provider.Services.Select(s => new ServiceViewModel
                {
                    ServiceId = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price
                }).ToList()
            };

            return View(viewModel);
        }


        public async Task<IActionResult> ServiceProviders(string searchQuery, string sortOrder)
        {
            var query = _context.ServiceProviders
                .Include(sp => sp.Services)
                .AsQueryable();

            
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(sp =>
                    sp.BusinessName.Contains(searchQuery) ||
                    sp.Services.Any(s => s.Name.Contains(searchQuery)));
            }

            
            query = sortOrder switch
            {
                "name_asc" => query.OrderBy(sp => sp.BusinessName),
                "city" => query.OrderBy(sp => sp.City),
                _ => query.OrderBy(sp => sp.BusinessName), 
            };

            var serviceProviders = await query
                .Select(sp => new ServiceProviderViewModel
                {
                    ServiceProviderId = sp.Id,
                    BusinessName = sp.BusinessName,
                    Address = sp.Address,
                    City = sp.City,
                    ZipCode = sp.ZipCode,
                    Services = sp.Services.Select(s => new ServiceViewModel
                    {
                        ServiceId = s.Id,
                        Name = s.Name,
                        Description = s.Description,
                        Price = s.Price
                    }).ToList()
                })
                .ToListAsync();

            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchQuery = searchQuery;

            return View(serviceProviders);
        }

        [Authorize]
        [HttpPost]

        public async Task<IActionResult> BookService(int id)
        {
            var userId = _userManager.GetUserId(User);
            var slot = await _context.ScheduleSlots
                .Include(s => s.Service)
                .ThenInclude(service => service.ServiceProvider) 
                .FirstOrDefaultAsync(s => s.Id == id);

            if (slot == null || slot.IsBooked)
            {
                TempData["ErrorMessage"] = "Termin nie jest dostępny.";
                return RedirectToAction("ServiceSchedule", new { id = slot.ServiceId });
            }

            
            var serviceProviderId = slot.Service.ServiceProvider?.Id;
            if (serviceProviderId == null)
            {
                TempData["ErrorMessage"] = "Nie można znaleźć usługodawcy dla tej usługi.";
                return RedirectToAction("ServiceSchedule", new { id = slot.ServiceId });
            }

            
            var reservation = new Reservation
            {
                CustomerId = userId,
                ServiceId = slot.ServiceId,
                ReservationDate = DateTime.UtcNow,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = ReservationStatus.Pending,
                ServiceProviderId = serviceProviderId.Value 
            };

           
            slot.IsBooked = true;

            _context.Reservations.Add(reservation);
            _context.Update(slot);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Rezerwacja zakończona pomyślnie!";
            return RedirectToAction("ServiceSchedule", new { id = slot.ServiceId });
        }



        public async Task<IActionResult> MyReservations()
        {
            var userId = _userManager.GetUserId(User);

            var reservations = await _context.Reservations
                .Include(r => r.Service)
                .ThenInclude(s => s.ServiceProvider)
                .Where(r => r.CustomerId == userId)
                .Select(r => new ReservationViewModel
                {
                    ReservationId = r.Id,
                    ServiceName = r.Service.Name,
                    ServiceProviderName = r.Service.ServiceProvider.BusinessName,
                    ReservationDate = r.ReservationDate,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    Status = r.Status.ToString()
                })
                .ToListAsync();

            return View(reservations);
        }
    }
}
