using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TurboReserve.Services;
using TurboReserve.Models;

namespace TurboReserve.Controllers
{
    [Authorize(Roles = "ServiceProvider")] 
    public class ServiceProvidersController : Controller
    {
        private readonly IServiceProviderService _serviceProviderService;

        public ServiceProvidersController(IServiceProviderService serviceProviderService)
        {
            _serviceProviderService = serviceProviderService;
        }

        public async Task<IActionResult> Index()
        {
            var providers = await _serviceProviderService.GetAllAsync();
            return View(providers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var provider = await _serviceProviderService.GetByIdAsync(id);
            if (provider == null)
            {
                return NotFound();
            }
            return View(provider);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.ServiceProvider provider)
        {
            if (ModelState.IsValid)
            {
                await _serviceProviderService.AddAsync(provider);
                return RedirectToAction(nameof(Index));
            }
            return View(provider);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var provider = await _serviceProviderService.GetByIdAsync(id);
            if (provider == null)
            {
                return NotFound();
            }
            return View(provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Models.ServiceProvider provider)
        {
            if (id != provider.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _serviceProviderService.UpdateAsync(provider);
                return RedirectToAction(nameof(Index));
            }
            return View(provider);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var provider = await _serviceProviderService.GetByIdAsync(id);
            if (provider == null)
            {
                return NotFound();
            }
            return View(provider);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _serviceProviderService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
