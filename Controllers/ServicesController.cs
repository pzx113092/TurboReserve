using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TurboReserve.Services;
using TurboReserve.Models;

namespace TurboReserve.Controllers
{
    [Authorize(Roles = "ServiceProvider")]
    public class ServicesController : Controller
    {
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            var services = await _serviceService.GetAllAsync();
            return View(services);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Service service)
        {
            if (ModelState.IsValid)
            {
                // W tym miejscu możesz wstawić logikę, aby przypisać ServiceProviderId do aktualnie zalogowanego usługodawcy
                // np. jeśli trzymasz Id usługodawcy w IdentityUserClaims lub powiązanej tabeli:
                // service.ServiceProviderId = ... (pobierz z kontekstu użytkownika)

                await _serviceService.AddAsync(service);
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.Service service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            var existing = await _serviceService.GetByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existing.Name = service.Name;
                existing.Description = service.Description;
                existing.Price = service.Price;

                await _serviceService.UpdateAsync(existing);
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }



        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            await _serviceService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
