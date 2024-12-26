using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TurboReserve.Data;
using TurboReserve.Models;

namespace TurboReserve.Services
{
    public class ReservationService : IReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.ServiceProvider)
                .Include(r => r.Service)
                .ToListAsync();
        }

        public async Task<Reservation> GetByIdAsync(int id)
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.ServiceProvider)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Reservation>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Customer)
                .Include(r => r.ServiceProvider)
                .Include(r => r.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> GetByServiceProviderIdAsync(int serviceProviderId)
        {
            return await _context.Reservations
                .Where(r => r.ServiceProviderId == serviceProviderId)
                .Include(r => r.Customer)
                .Include(r => r.ServiceProvider)
                .Include(r => r.Service)
                .ToListAsync();
        }
    }
}

