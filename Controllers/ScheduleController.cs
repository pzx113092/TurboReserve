using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve.Models;
using TurboReserve.Services;
using OfficeOpenXml;

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

        ViewBag.StartDate = startDate.ToString("dd.mm.yyyy");
        ViewBag.EndDate = endDate.ToString("dd.mm.yyyy");

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
            .OrderBy(slot => slot.StartTime) 
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

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var slot = await _context.ScheduleSlots
            .Include(s => s.Service)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (slot == null)
        {
            return NotFound();
        }

        if (slot.IsBooked)
        {
            TempData["ErrorMessage"] = "Nie można usunąć terminu, który ma przypisaną rezerwację.";
            return RedirectToAction(nameof(Index));
        }

        return View(slot);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var slot = await _context.ScheduleSlots.FirstOrDefaultAsync(s => s.Id == id);

        if (slot == null)
        {
            return NotFound();
        }

        if (slot.IsBooked)
        {
            TempData["ErrorMessage"] = "Nie można usunąć terminu, który ma przypisaną rezerwację.";
            return RedirectToAction(nameof(Index));
        }

        _context.ScheduleSlots.Remove(slot);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Termin został pomyślnie usunięty.";
        return RedirectToAction(nameof(Index));
    }



    [HttpGet]
    public IActionResult ImportExcel()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportExcel(IFormFile excelFile)
    {
        if (excelFile == null || excelFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Nie przesłano żadnego pliku.";
            return RedirectToAction(nameof(ImportExcel));
        }

        try
        {
            var userId = _userManager.GetUserId(User); 
            int serviceProviderId = await GetServiceProviderIdByUserId(userId); 

            using var stream = new MemoryStream();
            await excelFile.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0]; 

            var slots = new List<ScheduleSlot>();
            int maxSlotsPerImport = 100; // Ograniczenie maksymalnej liczby slotów na import
            int processedRows = 0;

            for (int row = 2; row <= worksheet.Dimension.End.Row && processedRows < maxSlotsPerImport; row++) 
            {
                var serviceId = int.Parse(worksheet.Cells[row, 1].Text);
                var startTime = DateTime.Parse(worksheet.Cells[row, 2].Text);
                var endTime = DateTime.Parse(worksheet.Cells[row, 3].Text);

                // Walidacja: Slot nie może być w przeszłości
                if (startTime < DateTime.Now)
                {
                    TempData["ErrorMessage"] = $"Wiersz {row}: Slot z wsteczną datą rozpoczęcia został pominięty.";
                    continue;
                }

                // Walidacja: StartTime musi być wcześniejszy niż EndTime
                if (startTime >= endTime)
                {
                    TempData["ErrorMessage"] = $"Wiersz {row}: Niepoprawny przedział czasu.";
                    continue;
                }

                slots.Add(new ScheduleSlot
                {
                    ServiceId = serviceId,
                    StartTime = startTime,
                    EndTime = endTime,
                    ServiceProviderId = serviceProviderId 
                });

                processedRows++;
            }

            if (slots.Count > 0)
            {
                _context.ScheduleSlots.AddRange(slots);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"{slots.Count} terminów zostało pomyślnie zaimportowanych.";
            }
            else
            {
                TempData["ErrorMessage"] = "Żadne sloty nie zostały zaimportowane.";
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Wystąpił błąd podczas przetwarzania pliku: " + ex.Message;
            return RedirectToAction(nameof(ImportExcel));
        }
    }


    [HttpGet]
    public async Task<IActionResult> GenerateTemplate()
    {
        // Pobranie ID zalogowanego użytkownika
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        var userId = _userManager.GetUserId(User);

        // Pobranie ID usługodawcy na podstawie użytkownika
        var serviceProviderId = await _context.ServiceProviders
            .Where(sp => sp.IdentityUserId == userId)
            .Select(sp => sp.Id)
            .FirstOrDefaultAsync();

        if (serviceProviderId == 0)
        {
            TempData["ErrorMessage"] = "Nie można znaleźć powiązanego usługodawcy.";
            return RedirectToAction(nameof(Index));
        }

        // Pobranie usług przypisanych do zalogowanego usługodawcy
        var services = await _context.Services
            .Where(s => s.ServiceProviderId == serviceProviderId)
            .Select(s => new { s.Id, s.Name })
            .ToListAsync();

        if (!services.Any())
        {
            TempData["ErrorMessage"] = "Nie znaleziono żadnych usług przypisanych do Twojego konta.";
            return RedirectToAction(nameof(Index));
        }

        using var package = new ExcelPackage();

        // Arkusz wzorca do wypełnienia
        var templateSheet = package.Workbook.Worksheets.Add("Wzorzec");
        templateSheet.Cells[1, 1].Value = "ServiceId";
        templateSheet.Cells[1, 2].Value = "StartTime";
        templateSheet.Cells[1, 3].Value = "EndTime";

        templateSheet.Cells[2, 1].Value = services.First().Id; // Przykładowy ServiceId
        templateSheet.Cells[2, 2].Value = DateTime.Today.AddHours(9).ToString("yyyy-MM-dd HH:mm");
        templateSheet.Cells[2, 3].Value = DateTime.Today.AddHours(10).ToString("yyyy-MM-dd HH:mm");

        templateSheet.Cells[1, 1, 1, 3].Style.Font.Bold = true; // Nagłówki pogrubione
        templateSheet.Columns[1].AutoFit();
        templateSheet.Columns[2].AutoFit();
        templateSheet.Columns[3].AutoFit();

        // Arkusz z dostępnymi usługami
        var servicesSheet = package.Workbook.Worksheets.Add("Dostępne Usługi");
        servicesSheet.Cells[1, 1].Value = "Id";
        servicesSheet.Cells[1, 2].Value = "Nazwa Usługi";

        for (int i = 0; i < services.Count; i++)
        {
            servicesSheet.Cells[i + 2, 1].Value = services[i].Id;
            servicesSheet.Cells[i + 2, 2].Value = services[i].Name;
        }

        servicesSheet.Cells[1, 1, 1, 2].Style.Font.Bold = true; // Nagłówki pogrubione
        servicesSheet.Columns[1].AutoFit();
        servicesSheet.Columns[2].AutoFit();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"Wzorzec_Slotow_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}

