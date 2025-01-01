using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve.Models;
using TurboReserve.Services;

[Authorize(Roles = "ServiceProvider")]
public class ScheduleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public ScheduleController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Create()
    {
        var userId = _userManager.GetUserId(User);
        var serviceProviderId = await GetServiceProviderIdByUserId(userId);

        var services = await _context.Services
        .Where(s => s.ServiceProviderId == serviceProviderId)
        .ToListAsync();
        ViewBag.Services = services;
        ViewBag.ServiceProviderId = serviceProviderId;

        return View();
    }

    // POST: Schedule/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ScheduleSlot model)
    {
        {
        
        foreach (var state in ModelState)
            {   
                foreach(var error in state.Value.Errors) {
                    Console.WriteLine($"Błąd w polu {state.Key}: {error.ErrorMessage}");
                    if (error.ErrorMessage.Equals("EndTime must be later than StartTime.")){
                        TempData["ErrorMessage"] = "Data początkowa nie może być późniejsza niż data końcowa!";
                        if(state.Value.Errors.Count > 1) return RedirectToAction(nameof(Index));
                    }
                }
            }
        }
        var userId = _userManager.GetUserId(User);
        int serviceProviderId = await GetServiceProviderIdByUserId(userId);
        model.ServiceProviderId = serviceProviderId;
        _context.ScheduleSlots.Add(model);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));

    }

    public async Task<IActionResult> ServiceSchedule(int id, DateTime? from, DateTime? to)
    {
        var startDate = from ?? DateTime.Today;
        var endDate = to ?? DateTime.Today.AddDays(7);

        var service = await _context.Services
            .Include(s => s.ScheduleSlots)
            .FirstOrDefaultAsync(s => s.Id == id);

        var viewModel = new ServiceScheduleViewModel
        {
            ServiceId = service.Id,
            ServiceName = service.Name,
            ScheduleSlots = service.ScheduleSlots
                .Where(slot => slot.StartTime.Date >= startDate && slot.StartTime.Date <= endDate)
                .Select(slot => new ScheduleSlotViewModel
                {
                    SlotId = slot.Id,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime
                }).ToList()
        };

        ViewBag.StartDate = startDate.ToString("yyyy-MM-dd");
        ViewBag.EndDate = endDate.ToString("yyyy-MM-dd");

        return View(viewModel);
    }

    [Authorize(Roles = "ServiceProvider")]
    public async Task<IActionResult> Index(DateTime? from, DateTime? to)
    {
        var userId = _userManager.GetUserId(User);


        var startDate = from ?? DateTime.Today;
        var endDate = to ?? DateTime.Today.AddDays(7);


        if (startDate > endDate)
        {
            TempData["ErrorMessage"] = "Data początkowa nie może być późniejsza niż data końcowa.";
            return RedirectToAction(nameof(Index));
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
