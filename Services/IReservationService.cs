﻿using TurboReserve.Models;

namespace TurboReserve.Services
{
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation> GetByIdAsync(int id);
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task DeleteAsync(int id);

        Task<IEnumerable<Reservation>> GetByCustomerIdAsync(string customerId);
        Task<IEnumerable<Reservation>> GetByServiceProviderIdAsync(int serviceProviderId);
    }
}
