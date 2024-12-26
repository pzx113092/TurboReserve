using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve.Models;

namespace TurboReserve.Services
{
    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;

        public ServiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Models.Service>> GetAllAsync()
        {
            return await _context.Services
                .Include(s => s.ServiceProvider)
                .ToListAsync();
        }

        public async Task<Models.Service> GetByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.ServiceProvider)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Models.Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Models.Service service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Models.Service>> GetByServiceProviderIdAsync(int serviceProviderId)
        {
            return await _context.Services
                .Where(s => s.ServiceProviderId == serviceProviderId)
                .Include(s => s.ServiceProvider)
                .ToListAsync();
        }
    }
}
