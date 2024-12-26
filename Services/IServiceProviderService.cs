using System.Collections.Generic;
using System.Threading.Tasks;
using TurboReserve.Models;

namespace TurboReserve.Services
{
    public interface IServiceProviderService
    {
        Task<IEnumerable<Models.ServiceProvider>> GetAllAsync();
        Task<Models.ServiceProvider> GetByIdAsync(int id);
        Task AddAsync(Models.ServiceProvider provider);
        Task UpdateAsync(Models.ServiceProvider provider);
        Task DeleteAsync(int id);
    }
}
