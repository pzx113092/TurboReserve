using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve.Models;

namespace TurboReserve.Services
{
    public class ServiceProvidersController : IServiceProviderService
    {
        private readonly ApplicationDbContext _context;

        public ServiceProvidersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.ServiceProvider>> GetAllAsync()
        {
            return await _context.ServiceProviders
                .Include(sp => sp.Services)
                .ToListAsync();
        }

        public async Task<Models.ServiceProvider> GetByIdAsync(int id)
        {
            return await _context.ServiceProviders
                .Include(sp => sp.Services)
                .FirstOrDefaultAsync(sp => sp.Id == id);
        }

        public async Task AddAsync(Models.ServiceProvider provider)
        {
            _context.ServiceProviders.Add(provider);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Models.ServiceProvider provider)
        {
            _context.ServiceProviders.Update(provider);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var provider = await _context.ServiceProviders.FindAsync(id);
            if (provider != null)
            {
                _context.ServiceProviders.Remove(provider);
                await _context.SaveChangesAsync();
            }
        }

        Task<IEnumerable<Models.ServiceProvider>> IServiceProviderService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<Models.ServiceProvider> IServiceProviderService.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}

